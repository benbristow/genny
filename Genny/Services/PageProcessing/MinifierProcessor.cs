using System.Text.RegularExpressions;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that minifies HTML content by removing unnecessary whitespace.
/// </summary>
public partial class MinifierProcessor : IPageProcessor
{
    [GeneratedRegex(@">\s+<", RegexOptions.Multiline)]
    private static partial Regex WhitespaceBetweenTagsRegex();

    [GeneratedRegex(@">\s+", RegexOptions.Multiline)]
    private static partial Regex WhitespaceAfterOpeningTagRegex();

    [GeneratedRegex(@"\s+<", RegexOptions.Multiline)]
    private static partial Regex WhitespaceBeforeClosingTagRegex();

    [GeneratedRegex(@"\s+", RegexOptions.Multiline)]
    private static partial Regex MultipleWhitespaceRegex();

    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        var result = context.Content;

        // Remove whitespace between HTML tags (e.g., ">  <" becomes "><")
        result = WhitespaceBetweenTagsRegex().Replace(result, "><");

        // Remove whitespace immediately after opening tags (e.g., ">  Text" becomes ">Text")
        result = WhitespaceAfterOpeningTagRegex().Replace(result, ">");

        // Remove whitespace immediately before closing tags (e.g., "Text  <" becomes "Text<")
        result = WhitespaceBeforeClosingTagRegex().Replace(result, "<");

        // Collapse multiple whitespace characters (spaces, tabs, newlines) to single spaces
        // This preserves single spaces in text content while removing excess whitespace
        result = MultipleWhitespaceRegex().Replace(result, " ");

        // Trim the entire result
        result = result.Trim();

        context.Content = result;
        return Task.FromResult(context);
    }
}
