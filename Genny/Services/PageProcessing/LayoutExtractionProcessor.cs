using System.Text.RegularExpressions;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that extracts the layout name from page content.
/// </summary>
public partial class LayoutExtractionProcessor : IPageProcessor
{
    private const string DefaultLayoutName = "default.html";

    [GeneratedRegex(@"<!--\s*layout:\s*([^\s]+)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex LayoutCommentRegex();

    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        // Look for <!-- layout: layoutname.html --> or similar
        var layoutMatch = LayoutCommentRegex().Match(context.OriginalPageContent);

        if (layoutMatch is { Success: true, Groups.Count: > 1 })
        {
            context.LayoutName = layoutMatch.Groups[1].Value.Trim();
        }
        else
        {
            context.LayoutName = DefaultLayoutName;
        }

        return Task.FromResult(context);
    }
}
