using Google.Protobuf.Collections;
using InnerTube.Exceptions;
using InnerTube.Models;
using InnerTube.Protobuf;
using InnerTube.Protobuf.Params;
using InnerTube.Protobuf.Responses;
using InnerTube.Renderers;

namespace InnerTube;

public class SimpleInnerTubeClient(InnerTubeConfiguration? config = null)
{
	public InnerTube InnerTube = new(config);

	public async Task<InnerTubePlayer> GetVideoPlayerAsync(string videoId, bool contentCheckOk, string language = "en", string region = "US")
	{
		// in the worst case scenario, this will do 4 http requests :3
		try
		{
			PlayerResponse player = await InnerTube.GetPlayerAsync(videoId, contentCheckOk, false, language, region);
			return new InnerTubePlayer(player, false);
		}
		catch (PlayerException e)
		{
			if (e.Code != PlayabilityStatus.Types.Status.LiveStreamOffline) throw;

			PlayerResponse player = await InnerTube.GetPlayerAsync(videoId, contentCheckOk, true, language, region);
			return new InnerTubePlayer(player, true);
		}
	}

	public async Task<InnerTubeVideo> GetVideoDetailsAsync(string videoId, bool contentCheckOk, string? playlistId,
		int? playlistIndex, string? playlistParams, string language = "en", string region = "US")
	{
		NextResponse next = await InnerTube.GetNextAsync(videoId, contentCheckOk, true, playlistId, playlistIndex,
			playlistParams, language, region);
		return new InnerTubeVideo(next);
	}

	public async Task<ContinuationResponse> ContinueVideoRecommendationsAsync(string continuationKey,
		string language = "en", string region = "US")
	{
		NextResponse next = await InnerTube.ContinueNextAsync(continuationKey, language, region);
		RepeatedField<RendererWrapper> allItems = next.OnResponseReceivedEndpoints[0].AppendContinuationItemsAction
			.ContinuationItems;
		IEnumerable<RendererWrapper> items = allItems.Where(x =>
			x.RendererCase != RendererWrapper.RendererOneofCase.ContinuationItemRenderer);
		ContinuationItemRenderer? continuation = allItems.LastOrDefault(x =>
			x.RendererCase == RendererWrapper.RendererOneofCase.ContinuationItemRenderer)?.ContinuationItemRenderer;
		return new ContinuationResponse
		{
			ContinuationToken = continuation?.ContinuationEndpoint.ContinuationCommand.Token,
			Results = Utils.ConvertRenderers(items)
		};
	}

	// doesn't take language/region because comments don't have anything to localize server side
	public async Task<ContinuationResponse> GetVideoCommentsAsync(string videoId,
		CommentsContext.Types.SortOrder sortOrder) =>
		await ContinueVideoCommentsAsync(Utils.PackCommentsContinuation(videoId, sortOrder));

	public async Task<ContinuationResponse> ContinueVideoCommentsAsync(string continuationToken)
	{
		NextResponse next = await InnerTube.ContinueNextAsync(continuationToken);
		RendererWrapper[]? continuationItems =
			(next.OnResponseReceivedEndpoints.LastOrDefault()?.ReloadContinuationItemsCommand?.ContinuationItems ??
			 next.OnResponseReceivedEndpoints.LastOrDefault()?.AppendContinuationItemsAction?.ContinuationItems)?
			.Where(x => x.RendererCase is RendererWrapper.RendererOneofCase.CommentThreadRenderer
				or RendererWrapper.RendererOneofCase.ContinuationItemRenderer)
			.ToArray();
		if (continuationItems == null) return new ContinuationResponse
		{
			ContinuationToken = null,
			Results = []
		};
		if (continuationItems[0].CommentThreadRenderer.Comment != null)
		{
			// CommentRenderer instead of ViewModels
			return new ContinuationResponse
			{
				ContinuationToken = continuationItems
					.LastOrDefault(x => x.RendererCase == RendererWrapper.RendererOneofCase.ContinuationItemRenderer)
					?.ContinuationItemRenderer.ContinuationEndpoint.ContinuationCommand.Token,
				Results = continuationItems
					.Where(x => x.RendererCase == RendererWrapper.RendererOneofCase.CommentThreadRenderer)
					.Select(x => new RendererContainer
					{
						Type = "comment",
						OriginalType = "commentThreadRenderer",
						Data = new CommentRendererData(x.CommentThreadRenderer)
					}).ToArray()
			};
		}

		// ViewModels <3
		Dictionary<string, Payload> mutations =
			next.FrameworkUpdates.EntityBatchUpdate.Mutations.ToDictionary(x => x.EntityKey, x => x.Payload);
		return new ContinuationResponse
		{
			ContinuationToken = continuationItems
				.LastOrDefault(x => x.RendererCase == RendererWrapper.RendererOneofCase.ContinuationItemRenderer)
				?.ContinuationItemRenderer.ContinuationEndpoint.ContinuationCommand.Token,
			Results = continuationItems
				.Where(x => x.RendererCase == RendererWrapper.RendererOneofCase.CommentThreadRenderer)
				.Select(x => new RendererContainer
				{
					Type = "comment",
					OriginalType = "commentThreadRenderer",
					Data = new CommentRendererData(
						x.CommentThreadRenderer,
						mutations[x.CommentThreadRenderer.CommentViewModel.CommentViewModel.CommentKey]
							.CommentEntityPayload,
						mutations[x.CommentThreadRenderer.CommentViewModel.CommentViewModel.ToolbarStateKey]
							.EngagementToolbarStateEntityPayload)
				}).ToArray()
		};
	}

	public async Task<InnerTubeChannel> GetChannelAsync(string channelId, ChannelTabs tabs = ChannelTabs.Featured,
		string language = "en", string region = "US")
	{
		BrowseResponse channel = await InnerTube.BrowseAsync(channelId, tabs.GetParams(), null, language, region);
		return new InnerTubeChannel(channel);
	}

	public async Task<InnerTubeChannel> GetChannelAsync(string channelId, string param, string language = "en",
		string region = "US")
	{
		BrowseResponse channel = await InnerTube.BrowseAsync(channelId, Utils.GetParamsFromChannelTabName(param), null,
			language, region);
		return new InnerTubeChannel(channel);
	}

	public async Task<ContinuationResponse> ContinueChannelAsync(string continuationToken, string language = "en",
		string region = "US")
	{
		BrowseResponse next = await InnerTube.ContinueBrowseAsync(continuationToken, language, region);
		RepeatedField<RendererWrapper> allItems = next.OnResponseReceivedActions.AppendContinuationItemsAction
			.ContinuationItems;
		IEnumerable<RendererWrapper> items = allItems.Where(x =>
			x.RendererCase != RendererWrapper.RendererOneofCase.ContinuationItemRenderer);
		ContinuationItemRenderer? continuation = allItems.LastOrDefault(x =>
			x.RendererCase == RendererWrapper.RendererOneofCase.ContinuationItemRenderer)?.ContinuationItemRenderer;
		return new ContinuationResponse
		{
			ContinuationToken = continuation?.ContinuationEndpoint.ContinuationCommand.Token,
			Results = Utils.ConvertRenderers(items)
		};
	}

	public async Task<InnerTubeChannel> SearchChannelAsync(string channelId, string query,
		string language = "en", string region = "US")
	{
		BrowseResponse channel =
			await InnerTube.BrowseAsync(channelId, "EgZzZWFyY2jyBgQKAloA", query, language, region);
		return new InnerTubeChannel(channel);
	}

	public async Task<InnerTubePlaylist> GetPlaylistAsync(string playlistId, bool includeUnavailable = false,
		PlaylistFilter filter = PlaylistFilter.All, string language = "en", string region = "US")
	{
		BrowseResponse playlist =
			await InnerTube.BrowseAsync(playlistId.StartsWith("PL") ? "VL" + playlistId : playlistId,
				Utils.PackPlaylistParams(includeUnavailable, filter), null, language, region);
		return new InnerTubePlaylist(playlist);
	}

	public async Task<ContinuationResponse> ContinuePlaylistAsync(string continuationToken, string language = "en",
		string region = "US")
	{
		BrowseResponse playlist = await InnerTube.ContinueBrowseAsync(continuationToken, language, region);
		IEnumerable<RendererWrapper> renderers = playlist.Contents.TwoColumnBrowseResultsRenderer.Tabs[0]
			                                         .TabRenderer.Content?
			                                         .ResultsContainer.Results[0].ItemSectionRenderer
			                                         .Contents[0].PlaylistVideoListRenderer?.Contents ??
		                                         playlist.Contents.TwoColumnBrowseResultsRenderer.Tabs[0]
			                                         .TabRenderer.Content?
			                                         .ResultsContainer.Results[0].ItemSectionRenderer
			                                         .Contents ??
		                                         playlist.OnResponseReceivedActions?
			                                         .AppendContinuationItemsAction?.ContinuationItems ??
		                                         [];
		RendererContainer[] items = Utils.ConvertRenderers(renderers);

		return new ContinuationResponse
		{
			ContinuationToken = (items.LastOrDefault(x => x.Type == "continuation")?.Data as ContinuationRendererData)
				?.ContinuationToken,
			Results = items.Where(x => x.Type != "continuation").ToArray()
		};
	}

	public async Task<InnerTubeSearchResults> SearchAsync(string query, SearchParams? param = null, string language = "en",
		string region = "US")
	{
		SearchResponse searchResponse = await InnerTube.SearchAsync(query, param, language, region);
		return new InnerTubeSearchResults(searchResponse);
	}

	public async Task<ContinuationResponse> ContinueSearchAsync(string continuationToken, string language = "en",
		string region = "US")
	{
		SearchResponse searchResponse = await InnerTube.ContinueSearchAsync(continuationToken, language, region);
		return new ContinuationResponse
		{
			ContinuationToken = searchResponse.OnResponseReceivedCommands.AppendContinuationItemsAction
				.ContinuationItems
				.LastOrDefault(x => x.RendererCase == RendererWrapper.RendererOneofCase.ContinuationItemRenderer)
				?.ContinuationItemRenderer.ContinuationEndpoint.ContinuationCommand.Token,
			Results = Utils.ConvertRenderers(
				searchResponse.OnResponseReceivedCommands.AppendContinuationItemsAction.ContinuationItems.SelectMany(
					x => x.ItemSectionRenderer?.Contents ?? []))
		};
	}
}