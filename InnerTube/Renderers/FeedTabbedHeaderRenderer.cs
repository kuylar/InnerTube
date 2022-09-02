﻿using System.Text;
using Newtonsoft.Json.Linq;

namespace InnerTube.Renderers;

public class FeedTabbedHeaderRenderer : IRenderer
{
	public string Type => "feedTabbedHeaderRenderer";

	public string Title { get; }

	public FeedTabbedHeaderRenderer(JToken renderer)
	{
		Title = Utils.ReadRuns(renderer.GetFromJsonPath<JArray>("title.runs")!);
	}

	public override string ToString() =>
		new StringBuilder()
			.AppendLine($"[{Type}] {Title}")
			.ToString();
}