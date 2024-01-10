using System.Text;
using InnerTube.Protobuf.Renderers;
using InnerTube.Protobuf.Requests;

namespace InnerTube.Tests;

public class PlayerTests
{
	private InnerTube _innerTube;

	[SetUp]
	public void Setup()
	{
		_innerTube = new InnerTube();
	}

	[TestCase("BaW_jenozKc", true, true, Description = "Load a video with an HLS manifest")]
	[TestCase("J6Ga4wciA2k", true, false, Description = "Load a video with the endscreen & info cards")]
	[TestCase("jfKfPfyJRdk", true, false, Description = "Load a livestream")]
	[TestCase("9gIXoaB-Jik", true, false, Description = "Video with WEBSITE endscreen item")]
	[TestCase("4ZX9T0kWb4Y", true, false, Description = "Video with multiple audio tracks")]
	[TestCase("-UBaW1OIgTo", true, false, Description = "EndScreenItem ctor")]
	public async Task GetPlayer(string videoId, bool contentCheckOk, bool includeHls)
	{
		PlayerResponse player = await _innerTube.GetPlayerAsync(videoId, contentCheckOk, includeHls);
		StringBuilder sb = new();

		sb.AppendLine("== DETAILS")
			.AppendLine("Id: " + player.VideoDetails.VideoId)
			.AppendLine("Title: " + player.VideoDetails.Title)
			.AppendLine("Author: " + player.VideoDetails.Author)
			.AppendLine("Keywords: " + string.Join(", ", player.VideoDetails.Keywords.Select(x => $"#{x}")))
			.AppendLine("ShortDescription: " + player.VideoDetails.ShortDescription.Split('\n')[0])
			//.AppendLine("Category: " + player.VideoDetails.Category)
			//.AppendLine("UploadDate: " + player.VideoDetails.UploadDate)
			//.AppendLine("PublishDate: " + player.VideoDetails.PublishDate)
			.AppendLine("Length: " + player.VideoDetails.LengthSeconds)
			//.AppendLine("IsLive: " + player.VideoDetails.IsLive)
			//.AppendLine("AllowRatings: " + player.VideoDetails.AllowRatings)
			//.AppendLine("IsFamilySafe: " + player.VideoDetails.IsFamilySafe)
			.AppendLine("Thumbnails: " + player.VideoDetails.Thumbnail.Thumbnails_.Count);

		//TODO: Storyboard
		//sb.AppendLine("== STORYBOARD")
		//	.AppendLine("RecommendedLevel: " + player.Storyboard.RecommendedLevel);
		//foreach ((int level, Uri? uri) in player.Storyboard.Levels) sb.AppendLine($"-> L{level}: {uri}");

		sb.AppendLine("== ENDSCREEN");
		if (player.Endscreen != null)
		{
			sb.AppendLine("Start: " + TimeSpan.FromMilliseconds(player.Endscreen.EndscreenRenderer.StartMs));
			foreach (EndscreenElementRenderer item in player.Endscreen.EndscreenRenderer.Elements.Select(x =>
				         x.EndscreenElementRenderer))
			{
				sb
					.AppendLine($"-> [{item.Style}] Endscreen item")
					.AppendLine("   Target: " + item.Endpoint)
					.AppendLine("   Title: " + item.Title)
					.AppendLine("   Image: " + item.Image.Thumbnails_.First().Url)
					.AppendLine("   Icon: " + item.Icon?.Thumbnails_.First().Url)
					.AppendLine("   Metadata: " + item.Metadata)
					.AppendLine("   Style: " + item.Style)
					.AppendLine("   AspectRatio: " + item.AspectRatio)
					.AppendLine("   Left: " + item.Left)
					.AppendLine("   Top: " + item.Top)
					.AppendLine("   Width: " + item.Width);
			}
		}

		sb.AppendLine("== CAPTIONS");
		if (player.Captions != null) // why doesnt protoc create a HasCaptions value????
			foreach (PlayerCaptionsTracklistRenderer.Types.Caption item in player.Captions.CaptionsTrackListRenderer
				         .Captions)
			{
				sb
					.AppendLine($"-> [{item.VssId}] ({item.Language}) {item.Name}")
					.AppendLine("   Url: " + item.BaseUrl)
					.AppendLine("   Kind: " + item.Kind);
			}

		sb.AppendLine("== FORMATS");
		foreach (Format f in player.StreamingData.Formats)
		{
			sb
				.AppendLine($"-> [{f.Itag}] {f.QualityLabel}")
				.AppendLine("   Bitrate: " + f.Bitrate)
				.AppendLine("   ContentLength: " + f.ContentLength)
				.AppendLine("   Fps: " + f.Fps)
				.AppendLine("   Height: " + f.Height)
				.AppendLine("   Width: " + f.Width)
				.AppendLine("   InitRange: " + f.InitRange)
				.AppendLine("   IndexRange: " + f.IndexRange)
				.AppendLine("   MimeType: " + f.Mime)
				.AppendLine("   Url: " + f.Url)
				.AppendLine("   Quality: " + f.Quality)
				//.AppendLine("   AudioQuality: " + f.AudioQuality)
				.AppendLine("   AudioSampleRate: " + f.AudioSampleRate)
				.AppendLine("   AudioChannels: " + f.AudioChannels)
				.AppendLine("   AudioTrack: " + (f.AudioTrack?.ToString() ?? "<no audio track>"));
		}

		sb.AppendLine("== ADAPTIVE FORMATS");
		foreach (Format f in player.StreamingData.AdaptiveFormats)
		{
			sb
				.AppendLine($"-> [{f.Itag}] {f.QualityLabel}")
				.AppendLine("   Bitrate: " + f.Bitrate)
				.AppendLine("   ContentLength: " + f.ContentLength)
				.AppendLine("   Fps: " + f.Fps)
				.AppendLine("   Height: " + f.Height)
				.AppendLine("   Width: " + f.Width)
				.AppendLine("   InitRange: " + f.InitRange)
				.AppendLine("   IndexRange: " + f.IndexRange)
				.AppendLine("   MimeType: " + f.Mime)
				.AppendLine("   Url: " + f.Url)
				.AppendLine("   Quality: " + f.Quality)
				//.AppendLine("   AudioQuality: " + f.AudioQuality)
				.AppendLine("   AudioSampleRate: " + f.AudioSampleRate)
				.AppendLine("   AudioChannels: " + f.AudioChannels)
				.AppendLine("   AudioTrack: " + (f.AudioTrack?.ToString() ?? "<no audio track>"));
		}

		sb.AppendLine("== OTHER")
			.AppendLine("ExpiresInSeconds: " + player.StreamingData.ExpiresInSeconds)
			.AppendLine("HlsManifestUrl: " + player.StreamingData.HlsManifestUrl)
			.AppendLine("DashManifestUrl: " + player.StreamingData.DashManifestUrl);


		Assert.Pass(sb.ToString());
	}

	[TestCase("V6kJKxvbgZ0", true, false, Description = "Age restricted video")]
	[TestCase("LACbVhgtx9I", false, false, Description = "Video that includes self-harm topics")]
	public void FailPlayer(string videoId, bool contentCheckOk, bool includeHls)
	{
		Assert.Catch(() =>
		{
			//InnerTubePlayer _ = _innerTube.GetPlayerAsync(videoId, contentCheckOk, includeHls).Result;
		});
	}

	[TestCase("BaW_jenozKc", Description = "Regular video")]
	[TestCase("V6kJKxvbgZ0", Description = "Age restricted video")]
	[TestCase("LACbVhgtx9I", Description = "Video that includes self-harm topics")]
	[TestCase("Atvsg_zogxo", Description = "something broke CompactPlaylistRenderer")]
	[TestCase("t6cZn-Fvwa0", Description = "Video with comments disabled")]
	[TestCase("jPhJbKBuNnA", Description = "Video with watchEndpoint in attributedDescription")]
	[TestCase("UoBFuLMlDkw", Description = "Video with more special stuff in attributedDescription")]
	[TestCase("llrBX6FpMpM", Description = "compactMovieRenderer")]
	[TestCase("jUUe6TuRlgU", Description = "Chapters")]
	public async Task GetVideoNext(string videoId)
	{
		/*
		InnerTubeNextResponse next = await _innerTube.GetVideoAsync(videoId);

		StringBuilder sb = new();

		sb.AppendLine("== DETAILS")
			.AppendLine("Id: " + next.Id)
			.AppendLine("Title: " + next.Title)
			.AppendLine("Channel: " + next.Channel)
			.AppendLine("DateText: " + next.DateText)
			.AppendLine("ViewCount: " + next.ViewCount)
			.AppendLine("LikeCount: " + next.LikeCount)
			.AppendLine("Description:\n" + string.Join('\n', next.Description.Split("\n").Select(x => $"\t{x}")));

		sb.AppendLine("\n== CHAPTERS");
		if (next.Chapters != null)
		{
			foreach (ChapterRenderer chapter in next.Chapters)
				sb.AppendLine($"- [{TimeSpan.FromMilliseconds(chapter.TimeRangeStartMillis)}] {chapter.Title}");
		}
		else
		{
			sb.AppendLine("No chapters available");
		}

		sb.AppendLine("\n== COMMENTS")
			.AppendLine("CommentCount: " + next.CommentCount)
			.AppendLine("CommentsContinuation: " + next.CommentsContinuation);

		sb.AppendLine("\n== RECOMMENDED");
		foreach (IRenderer renderer in next.Recommended)
		{
			sb.AppendLine("->\t" + string.Join("\n\t",
				(renderer.ToString() ?? "UNKNOWN RENDERER " + renderer.Type).Split("\n")));
		}

		Assert.Pass(sb.ToString());
		*/
	}

	[TestCase("3BR7-AzE2dQ", "OLAK5uy_l6pEkEJgy577R-aDlJ3Gkp5rmlgIOu8bc", null, null)]
	[TestCase("o0tky2O8NlY", "OLAK5uy_l6pEkEJgy577R-aDlJ3Gkp5rmlgIOu8bc", null, null)]
	[TestCase("NZwS7Cja6oE", "PLv3TTBr1W_9tppikBxAE_G6qjWdBljBHJ", null, null)]
	[TestCase("k_nLHgIM4yE", "PLv3TTBr1W_9tppikBxAE_G6qjWdBljBHJ", null, null)]
	public async Task GetVideoNextWithPlaylist(string videoId, string playlistId, int? playlistIndex,
		string? playlistParams)
	{
		/*
		InnerTubeNextResponse next = await _innerTube.GetVideoAsync(videoId, playlistId, playlistIndex, playlistParams);
		if (next.Playlist is null)
		{
			Assert.Fail("Playlist is null");
			return;
		}

		StringBuilder sb = new();

		sb.AppendLine($"[{next.Playlist.PlaylistId}] {next.Playlist.Title}")
			.AppendLine($"{next.Playlist.Channel}")
			.AppendLine(
				$"{next.Playlist.CurrentIndex} ({next.Playlist.LocalCurrentIndex}) / {next.Playlist.TotalVideos}")
			.AppendLine($"IsCourse: {next.Playlist.IsCourse}")
			.AppendLine($"IsInfinite: {next.Playlist.IsInfinite}");

		sb.AppendLine()
			.AppendLine("== VIDEOS");

		foreach (PlaylistPanelVideoRenderer video in next.Playlist.Videos)
			sb.AppendLine(video.ToString());

		Assert.Pass(sb.ToString());
		*/
	}

	[TestCase("1234567890a", Description = "An ID I just made up")]
	[TestCase("a62882basgl", Description = "Another ID I just made up")]
	[TestCase("32nkdvLq3oQ", Description = "A deleted video")]
	[TestCase("mVp-gQuCJI8", Description = "A private video")]
	public async Task DontGetVideoNext(string videoId)
	{
		/*
		try
		{
			await _innerTube.GetVideoAsync(videoId);
		}
		catch (InnerTubeException e)
		{
			Assert.Pass($"Exception thrown: [{e.GetType().Name}] {e.Message}");
		}
		catch (Exception e)
		{
			Assert.Fail("Wrong type of exception has been thrown\n" + e);
		}

		Assert.Fail("Didn't throw an exception");
		*/
	}

	[TestCase("BaW_jenozKc", Description = "Regular video comments")]
	[TestCase(
		"Eg0SC3F1STZnNEhwZVBjGAYyVSIuIgtxdUk2ZzRIcGVQYzAAeAKqAhpVZ3p3MnBIQXR1VW9xamRLbUtWNEFhQUJBZzABQiFlbmdhZ2VtZW50LXBhbmVsLWNvbW1lbnRzLXNlY3Rpb24%3D",
		Description = "Contains pinned & hearted comments")]
	[TestCase("Eg0SC2tZd0Ita1p5TlU0GAYyJSIRIgtrWXdCLWtaeU5VNDAAeAJCEGNvbW1lbnRzLXNlY3Rpb24%3D",
		Description = "Contains authors with badges")]
	[TestCase("5UCz9i2K9gY", Description = "Has unescaped HTML tags")]
	public async Task GetVideoComments(string videoId)
	{
		/*
		InnerTubeContinuationResponse comments;
		if (videoId.Length == 11)
		{
			InnerTubeNextResponse next = await _innerTube.GetVideoAsync(videoId);
			if (next.CommentsContinuation is null) Assert.Fail("Video did not contain a comment continuation token");
			comments = await _innerTube.GetVideoCommentsAsync(next.CommentsContinuation!);
		}
		else
		{
			comments = await _innerTube.GetVideoCommentsAsync(videoId!);
		}

		StringBuilder sb = new();

		foreach (IRenderer renderer in comments.Contents) sb.AppendLine(renderer.ToString());

		sb.AppendLine($"\nContinuation: {comments.Continuation?.Substring(0, 20)}...");

		Assert.Pass(sb.ToString());
		*/
	}

	[TestCase("BaW_jenozKc", Description = "Regular video comments")]
	public async Task GetVideoCommentsProtobuf(string videoId)
	{
		/*
		InnerTubeContinuationResponse comments =
			await _innerTube.GetVideoCommentsAsync(videoId, CommentsContext.Types.SortOrder.TopComments);

		StringBuilder sb = new();
		foreach (IRenderer renderer in comments.Contents) sb.AppendLine(renderer.ToString());
		sb.AppendLine($"\nContinuation: {comments.Continuation?[..20]}...");

		Assert.Pass(sb.ToString());
		*/
	}


	[TestCase("astISOttCQ0", Description = "Video with comments disabled")]
	public void DontGetVideoCommentsProtobuf(string videoId)
	{
		/*
		Assert.Catch(() =>
		{
			_ = _innerTube.GetVideoCommentsAsync(videoId, CommentsContext.Types.SortOrder.TopComments).Result;
		});
		*/
	}

	[TestCase("there's no way they will accept this as a continuation key", Description = "Self explanatory")]
	public async Task DontGetVideoComments(string continuationToken)
	{
		/*
		try
		{
			await _innerTube.GetVideoCommentsAsync(continuationToken);
		}
		catch (InnerTubeException e)
		{
			Assert.Pass($"Exception thrown: [{e.GetType().Name}] {e.Message}");
		}
		catch (ArgumentException e)
		{
			Assert.Pass($"Exception thrown: [{e.GetType().Name}] {e.Message}");
		}
		catch (Exception e)
		{
			Assert.Fail("Wrong type of exception has been thrown\n" + e);
		}

		Assert.Fail("Didn't throw an exception");
		*/
	}
}