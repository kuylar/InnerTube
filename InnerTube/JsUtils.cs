using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json.Linq;

namespace InnerTube;

public class JsUtils
{
	public int? SignatureTimestamp { get; private set; }
	private V8Script? cachedScript;
	private HttpClient client = new();
	private V8Runtime v8 = new();

	private readonly Regex jsSrcRegex =
		new(@"src=""(?<url>\/s\/player\/(?<version>[^\/]+)\/player_ias[^\/]+\/.+?\/base.js)""");
	private readonly Regex signatureTimestampRegex = new("(?:signatureTimestamp|sts):(\\d{5})");
	private readonly Regex descramblerFunctionRegex = new(@"(?:[^=]+)=function\((\w)\){\w=\w.split\(""""\);(.+?)return \w.join");
	private readonly Regex nParamFunctionNameRegex =
		new(@"\.get\(""n""\)\)&&\([a-zA-Z0-9$_]=([a-zA-Z0-9$_]+)(?:\[(\d+)])?\([a-zA-Z0-9$_]\)");

	private const string SCRIPT_TEMPLATE =
		"function descrambleSignatureCipher(scrambled){%%SIGNATUREPARAM%%=scrambled.split(\"\");%%SIGNATURE%%;return %%SIGNATUREPARAM%%.join(\"\")}\nfunction descrambleNParam%%NPARAM%%";

	public async Task LoadLatestJs(string videoId)
	{
		if (cachedScript != null) return;

		// get the player HTML page so we can get an up-to-date JS URL
		HttpResponseMessage playerPageResponse =
			await client.GetAsync("https://youtube.com/watch?hl=en&gl=US&v=" + videoId);
		string playerPageHtml = await playerPageResponse.Content.ReadAsStringAsync();

		// find the JS URL
		Match jsMatch = jsSrcRegex.Match(playerPageHtml);

		string jsUrl = "https://www.youtube.com" + jsMatch.Groups["url"].Value;
		HttpResponseMessage jsResponse = await client.GetAsync(jsUrl);
		string js = await jsResponse.Content.ReadAsStringAsync();

		// get the signature cipher
		SignatureTimestamp = int.Parse(signatureTimestampRegex.Match(js).Groups[1].Value);

		// parse JS with Regex (to find the signatureCipher descrambler function)
		Match descrambleFunctionMatch = descramblerFunctionRegex.Match(js);

		// parse JS with Regex (to find the object that contains the a.reverse a.splice etc.)
		string descramblerMethodContainerObjectName =
			descrambleFunctionMatch.Groups[2].Value.Split(".", 2)[0];
		Match descramblerObjectMatch =
			new Regex(descramblerMethodContainerObjectName + "={(.+?)};").Match(js.ReplaceLineEndings(""));
		string descramblerObject = descramblerObjectMatch.Groups[1].Value;

		// parse JS with Regex (to find the descrambler function for the n query param)
		Match nParamFunctionNameMatch = nParamFunctionNameRegex.Match(js);

		string nParamFunctionName = nParamFunctionNameMatch.Groups[1].Value;
		if (!string.IsNullOrWhiteSpace(nParamFunctionNameMatch.Groups[2].Value))
		{
			int arrayIndex = int.Parse(nParamFunctionNameMatch.Groups[2].Value);
			Match match = new Regex($@"var {Regex.Escape(nParamFunctionName)}\s*=\s*\[(.+?)][;,]").Match(js);
			nParamFunctionName = match.Groups[1].Value.Split(",")[arrayIndex];
		}

		Match nParamFunctionBodyMatch = new Regex(
			$@"{Regex.Escape(nParamFunctionName)}=\s*function([\S\s]*?\}}\s*return [\w$]+?\.join\(""""\)\s*\}};)",
			RegexOptions.Singleline).Match(js);

		string finalJs =
			$"const {descramblerMethodContainerObjectName}={{{descramblerObject}}}\n" + SCRIPT_TEMPLATE
				.Replace("%%SIGNATUREPARAM%%", descrambleFunctionMatch.Groups[1].Value)
				.Replace("%%SIGNATURE%%", descrambleFunctionMatch.Groups[2].Value)
				.Replace("%%NPARAM%%", nParamFunctionBodyMatch.Groups[1].Value);
		cachedScript = v8.Compile(finalJs);
	}

	public string DescrambleUrl(JObject format)
	{
		string url;
		if (!format.ContainsKey("signatureCipher"))
		{
			url = UnthrottleUrl(format.GetValue("url")!.Value<string>()!);
		}
		else
		{
			NameValueCollection signatureCipher = HttpUtility.ParseQueryString(
				format.GetValue("signatureCipher")!.Value<string>()!);

			V8ScriptEngine engine = v8.CreateScriptEngine();
			engine.Execute(cachedScript);
			string descrambled =
				(engine.Evaluate($"descrambleSignatureCipher(\"{signatureCipher.Get("s")!}\")") as string)!;
			url =
				UnthrottleUrl($"{signatureCipher.Get("url")!}&{signatureCipher.Get("sp")}={HttpUtility.UrlEncode(descrambled)}", engine);
			engine.Dispose();
		}

		return url;
	}

	private string UnthrottleUrl(string url)
	{
		if (!url.Contains("&n=")) return url;
		V8ScriptEngine engine = v8.CreateScriptEngine();
		engine.Execute(cachedScript);
		string res = UnthrottleUrl(url, engine);
		engine.Dispose();
		return res;
	}

	private string UnthrottleUrl(string url, V8ScriptEngine engine)
	{
		NameValueCollection query = HttpUtility.ParseQueryString(url.Split("?")[1]);
		if (query.Get("n") == null) return url;
		string descrambled = (engine.Evaluate($"descrambleNParam(\"{query.Get("n")!}\")") as string)!;
		return url.Replace("&n=" + query.Get("n"), "&n=" + descrambled);
	}
}