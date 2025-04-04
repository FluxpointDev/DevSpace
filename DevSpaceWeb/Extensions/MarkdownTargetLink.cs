﻿using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.TargetLinkExtensions;

public class TargetLinkExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {

    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            LinkInlineRenderer? linkInlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
            linkInlineRenderer?.TryWriters.Add(TryLinkInlineRenderer);
        }
    }

    private bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
    {
        TryAddTarget(linkInline.Url, linkInline);
        return false;
    }

    private void TryAddTarget(string? url, MarkdownObject obj)
    {
        if (string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            obj.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }
    }
}
