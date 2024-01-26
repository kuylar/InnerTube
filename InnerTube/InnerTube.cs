﻿using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Google.Protobuf;
using InnerTube.Exceptions;
using InnerTube.Protobuf.Requests;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace InnerTube;

/// <summary>
/// The InnerTube client.
/// </summary>
public class InnerTube
{
	internal readonly HttpClient HttpClient = new();
	internal readonly MemoryCache PlayerCache;
	internal readonly string ApiKey;
	internal readonly InnerTubeAuthorization? Authorization;

	/// <summary>
	/// Initializes a new instance of InnerTube client.
	/// </summary>
	/// <param name="config">Configuration parameters</param>
	public InnerTube(InnerTubeConfiguration? config = null)
	{
		config ??= new InnerTubeConfiguration();
		ApiKey = config.ApiKey;
		Authorization = config.Authorization;
		PlayerCache = new MemoryCache(new MemoryCacheOptions
		{
			ExpirationScanFrequency = config.CacheExpirationPollingInterval,
			SizeLimit = config.CacheSize
		});
	}

	private async Task<byte[]> MakeRequest(RequestClient client, string endpoint, InnerTubeRequest postData,
		string language, string region, bool authorized = false)
	{
		string url = $"https://youtubei.googleapis.com/youtubei/v1/{endpoint}";
		url += "?alt=proto";
		if (!authorized || Authorization?.Type != AuthorizationType.REFRESH_TOKEN)
			url += $"&key={ApiKey}";


		HttpRequestMessage hrm = new(HttpMethod.Post, url);

		byte[] buffer = Encoding.UTF8.GetBytes(postData.GetJson(client, language, region));
		ByteArrayContent byteContent = new(buffer);
		byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		hrm.Content = byteContent;

		if (authorized && Authorization is not null)
		{
			if (Authorization.Type == AuthorizationType.SAPISID)
				hrm.Headers.Add("Cookie", Authorization.GenerateCookieHeader());
			hrm.Headers.Add("Authorization", Authorization.GenerateAuthHeader());
		}

		hrm.Headers.Add("X-Youtube-Client-Name", ((int)client).ToString());
		hrm.Headers.Add("X-Youtube-Client-Version", client switch
		{
			RequestClient.WEB => "2.20220809.02.00",
			RequestClient.ANDROID => "17.31.4",
			RequestClient.IOS => "17.31.4",
			var _ => ""
		});
		//hrm.Headers.Add("Origin", "https://www.youtube.com");
		if (client == RequestClient.ANDROID)
			hrm.Headers.Add("User-Agent", "com.google.android.youtube/17.31.35 (Linux; U; Android 11) gzip");

		HttpResponseMessage ytPlayerRequest = await HttpClient.SendAsync(hrm);
		if (!ytPlayerRequest.IsSuccessStatusCode)
			throw new RequestException(ytPlayerRequest.StatusCode, await ytPlayerRequest.Content.ReadAsStringAsync());
		return await ytPlayerRequest.Content.ReadAsByteArrayAsync();
	}

	/// <summary>
	/// Gets the player data of a video
	/// </summary>
	/// <param name="videoId">ID of the video</param>
	/// <param name="contentCheckOk">Set to true if you want to skip the content warnings (suicide, self-harm etc.)</param>
	/// <param name="language">Language of the content</param>
	/// <param name="region">Region of the content</param>
	public async Task<PlayerResponse> GetPlayerAsync(string videoId, bool contentCheckOk = false,
		string language = "en", string region = "US")
	{
		string cacheId = $"{videoId}_({language}_{region})";

		if (PlayerCache.TryGetValue(cacheId, out PlayerResponse? cachedPlayer)) return cachedPlayer!;

		Task<PlayerResponse>[] tasks =
		{
			GetPlayerObjectAsync(videoId, contentCheckOk, language, region, RequestClient.WEB),
			GetPlayerObjectAsync(videoId, contentCheckOk, language, region, RequestClient.ANDROID),
			GetPlayerObjectAsync(videoId, contentCheckOk, language, region, RequestClient.IOS)
		};
		Task.WaitAll(tasks.Cast<Task>().ToArray());
		PlayerResponse[] players = tasks.Select(x => x.Result).ToArray();

		if (players[0].PlayabilityStatus.Status != PlayabilityStatus.Types.Status.Ok)
			throw new PlayerException(players[1].PlayabilityStatus.Status, players[1].PlayabilityStatus.Reason,
				players[1].PlayabilityStatus.Subreason);

		players[0].StreamingData = players[1].StreamingData;
		if (players[0].StreamingData != null && players[2].StreamingData != null &&
		    players[0].StreamingData.HasHlsManifestUrl && players[2].StreamingData.HasHlsManifestUrl)
			players[0].StreamingData.HlsManifestUrl = players[2].StreamingData.HlsManifestUrl;

		PlayerCache.Set(cacheId, players[0], new MemoryCacheEntryOptions
		{
			Size = 1,
			SlidingExpiration = TimeSpan.FromSeconds(Math.Max(600, players[0].VideoDetails.LengthSeconds)),
			AbsoluteExpirationRelativeToNow =
				TimeSpan.FromSeconds(Math.Max(3600, players[0].StreamingData.ExpiresInSeconds - players[0].VideoDetails.LengthSeconds))
		});
		return players[0];
	}

	// instead of trying hours to find a protobuf compilation for
	// the params to get endscreen, cards, storyboards and non 403'ing
	// video data at the same time i decided to just do the request
	// in all clients. if someone finds a protobuf string that returns
	// all those on the ANDROID client pls pr <3
	private async Task<PlayerResponse> GetPlayerObjectAsync(string videoId, bool contentCheckOk, string language,
		string region, RequestClient client)
	{
		InnerTubeRequest postData = new InnerTubeRequest()
			.AddValue("videoId", videoId)
			.AddValue("contentCheckOk", contentCheckOk)
			.AddValue("racyCheckOk", contentCheckOk);

		if (client == RequestClient.ANDROID)
			postData.AddValue("params", "CgIQBg");

		return PlayerResponse.Parser.ParseFrom(await MakeRequest(client, "player", postData,
			language, region, true));
	}
}