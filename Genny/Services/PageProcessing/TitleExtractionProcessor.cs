using System.Text.RegularExpressions;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that extracts the page title from page content.
/// </summary>
public partial class TitleExtractionProcessor : IPageProcessor
{
    [GeneratedRegex(@"<!--\s*title:\s*(.+?)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex TitleCommentRegex();

    [GeneratedRegex(@"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TitleTagRegex();

    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        // First, try to extract from <!-- title: Page Title --> comment
        var titleCommentMatch = TitleCommentRegex().Match(context.OriginalPageContent);

        if (titleCommentMatch is { Success: true, Groups.Count: > 1 })
        {
            context.PageTitle = titleCommentMatch.Groups[1].Value.Trim();
            return Task.FromResult(context);
        }

        // Second, try to extract from <title> tag
        var titleTagMatch = TitleTagRegex().Match(context.OriginalPageContent);

        if (titleTagMatch is { Success: true, Groups.Count: > 1 })
        {
            context.PageTitle = titleTagMatch.Groups[1].Value.Trim();
            return Task.FromResult(context);
        }

        // No title found, keep empty string
        return Task.FromResult(context);
    }
}
