using System.Text.RegularExpressions;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that extracts all metadata (year, epoch, title, layout) from page content.
/// </summary>
public partial class MetadataExtractionProcessor : IPageProcessor
{
    private const string DefaultLayoutName = "default.html";

    [GeneratedRegex(@"<!--\s*title:\s*(.+?)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex TitleCommentRegex();

    [GeneratedRegex(@"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TitleTagRegex();

    [GeneratedRegex(@"<!--\s*layout:\s*([^\s]+)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex LayoutCommentRegex();

    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        // Extract year and epoch
        context.CurrentYear = DateTime.Now.Year.ToString();
        context.CurrentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        // Extract title from <!-- title: Page Title --> comment or <title> tag
        var titleCommentMatch = TitleCommentRegex().Match(context.OriginalPageContent);
        if (titleCommentMatch is { Success: true, Groups.Count: > 1 })
        {
            context.PageTitle = titleCommentMatch.Groups[1].Value.Trim();
        }
        else
        {
            var titleTagMatch = TitleTagRegex().Match(context.OriginalPageContent);
            if (titleTagMatch is { Success: true, Groups.Count: > 1 })
            {
                context.PageTitle = titleTagMatch.Groups[1].Value.Trim();
            }
        }

        // Extract layout name from <!-- layout: layoutname.html --> comment
        var layoutMatch = LayoutCommentRegex().Match(context.OriginalPageContent);
        context.LayoutName = layoutMatch is { Success: true, Groups.Count: > 1 } 
            ? layoutMatch.Groups[1].Value.Trim() 
            : DefaultLayoutName;

        return Task.FromResult(context);
    }
}
