using System.Text.RegularExpressions;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that removes layout and title comments from page content.
/// </summary>
public partial class CommentRemovalProcessor : IPageProcessor
{
    [GeneratedRegex(@"\s*<!--\s*layout:\s*[^\s]+\s*-->\s*", RegexOptions.IgnoreCase)]
    private static partial Regex LayoutCommentRemovalRegex();

    [GeneratedRegex(@"\s*<!--\s*title:\s*.+?\s*-->\s*", RegexOptions.IgnoreCase)]
    private static partial Regex TitleCommentRemovalRegex();

    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        // Remove layout comments: <!-- layout: layoutname.html --> (including surrounding whitespace)
        var result = LayoutCommentRemovalRegex().Replace(context.Content, string.Empty);

        // Remove title comments: <!-- title: Page Title --> (including surrounding whitespace)
        result = TitleCommentRemovalRegex().Replace(result, string.Empty);

        context.Content = result;
        // Set PageBody after comments are removed (used for {{content}} placeholder)
        context.PageBody = result;
        
        return Task.FromResult(context);
    }
}
