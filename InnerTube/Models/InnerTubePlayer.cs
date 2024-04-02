using System.Globalization;
using Newtonsoft.Json.Linq;

namespace InnerTube;

public class InnerTubePlayer
{
	public VideoDetails Details { get; }
	public VideoEndscreen Endscreen { get; }
	public VideoStoryboard Storyboard { get; }
	public IEnumerable<VideoCaption> Captions { get; }
	public IEnumerable<Format> Formats { get; }
	public IEnumerable<Format> AdaptiveFormats { get; }
	public int ExpiresInSeconds { get; }
	public string? HlsManifestUrl { get; }
	public string? DashManifestUrl { get; }

	public InnerTubePlayer(JObject playerResponse, JObject? hlsResponse, JsUtils jsUtils)
	{
		Details = new VideoDetails
		{
			Id = playerResponse.GetFromJsonPath<string>("videoDetails.videoId")!,
			Title = playerResponse.GetFromJsonPath<string>("videoDetails.title")!,
			Author = new Channel
			{
				Id = playerResponse.GetFromJsonPath<string>("videoDetails.channelId")!,
				Title = playerResponse.GetFromJsonPath<string>("videoDetails.author")!
			},
			Keywords = playerResponse.GetFromJsonPath<string[]>("videoDetails.keywords") ?? Array.Empty<string>(),
			ShortDescription = playerResponse.GetFromJsonPath<string>("videoDetails.shortDescription")!,
			Category = playerResponse.GetFromJsonPath<string>("videoDetails.category")!,
			UploadDate =
				DateTimeOffset.Parse(
					playerResponse.GetFromJsonPath<string>("microformat.playerMicroformatRenderer.uploadDate")!,
					CultureInfo.InvariantCulture),
			PublishDate =
				DateTimeOffset.Parse(
					playerResponse.GetFromJsonPath<string>("microformat.playerMicroformatRenderer.publishDate")!,
					CultureInfo.InvariantCulture),
			Length = TimeSpan.FromSeconds(
				long.Parse(playerResponse.GetFromJsonPath<string>("videoDetails.lengthSeconds")!)),
			IsLive = playerResponse.GetFromJsonPath<bool>("videoDetails.isLiveContent")!,
			AllowRatings = playerResponse.GetFromJsonPath<bool>("videoDetails.allowRatings"),
			IsFamilySafe = playerResponse.GetFromJsonPath<bool>("microformat.playerMicroformatRenderer.isFamilySafe")!,
			Thumbnails =
				Utils.GetThumbnails(
					playerResponse.GetFromJsonPath<JArray>(
						"microformat.playerMicroformatRenderer.thumbnail.thumbnails"))
		};
		Endscreen = new VideoEndscreen
		{
			Items = playerResponse.GetFromJsonPath<JArray>("endscreen.endscreenRenderer.elements")
				?.Select(x => new EndScreenItem(x["endscreenElementRenderer"]!)) ?? Array.Empty<EndScreenItem>(),
			StartMs = long.Parse(playerResponse.GetFromJsonPath<string>("endscreen.endscreenRenderer.startMs") ?? "0")
		};
		Storyboard = new VideoStoryboard
		{
			RecommendedLevel =
				playerResponse.GetFromJsonPath<int>("storyboards.playerStoryboardSpecRenderer.recommendedLevel"),
			Levels = Utils.GetLevelsFromStoryboardSpec(
				playerResponse.GetFromJsonPath<string>("storyboards.playerStoryboardSpecRenderer.spec"),
				long.Parse(playerResponse.GetFromJsonPath<string>("videoDetails.lengthSeconds")!))
		};
		Captions = playerResponse.GetFromJsonPath<JArray>("captions.playerCaptionsTracklistRenderer.captionTracks")?
			.Select(x => new VideoCaption
			{
				VssId = x["vssId"]?.ToString() ?? x["languageCode"]!.ToString(),
				LanguageCode = x["languageCode"]!.ToString(),
				Label = Utils.ReadText(x["name"]!.ToObject<JObject>()!),
				BaseUrl = new Uri(x["baseUrl"]!.ToString())
			}) ?? Array.Empty<VideoCaption>();
		Formats = playerResponse.GetFromJsonPath<JArray>("streamingData.formats")?.Select(x => new Format(x, jsUtils)) ??
		          Array.Empty<Format>();
		AdaptiveFormats =
			playerResponse.GetFromJsonPath<JArray>("streamingData.adaptiveFormats")?.Select(x => new Format(x, jsUtils)) ??
			Array.Empty<Format>();
		ExpiresInSeconds = playerResponse["streamingData"]?["expiresInSeconds"]?.ToObject<int>() ?? 0;
		// for live videos, the web client might get the hls/dash url
		HlsManifestUrl = hlsResponse?["streamingData"]?["hlsManifestUrl"]?.ToString() ??
		                 playerResponse?["streamingData"]?["hlsManifestUrl"]?.ToString();
		DashManifestUrl = hlsResponse?["streamingData"]?["dashManifestUrl"]?.ToString() ??
		                  playerResponse?["streamingData"]?["dashManifestUrl"]?.ToString();
	}

	public class VideoDetails
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public Channel Author { get; set; }
		public string[] Keywords { get; set; }
		public string ShortDescription { get; set; }
		public string Category { get; set; }
		public DateTimeOffset PublishDate { get; set; }
		public DateTimeOffset UploadDate { get; set; }
		public TimeSpan Length { get; set; }
		public bool IsLive { get; set; }
		public bool AllowRatings { get; set; }
		public bool IsFamilySafe { get; set; }
		public Thumbnail[] Thumbnails { get; set; }
	}

	public class VideoCaption
	{
		public string VssId { get; set; }
		public string LanguageCode { get; set; }
		public string Label { get; set; }
		public Uri BaseUrl { get; set; }
		public bool IsAutomaticCaption => VssId[0] == 'a';
	}

	public class VideoEndscreen
	{
		public IEnumerable<EndScreenItem> Items { get; set; }
		public long StartMs { get; set; }
	}

	public class VideoStoryboard
	{
		public int RecommendedLevel { get; set; }
		public Dictionary<int, Uri> Levels { get; set; }
	}
}