using System.Text.RegularExpressions;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that replaces placeholder values in content.
/// </summary>
public partial class PlaceholderReplacementProcessor : IPageProcessor
{
    [GeneratedRegex(@"\{\{\s*content\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex ContentPlaceholderRegex();

    [GeneratedRegex(@"\{\{\s*title\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex TitlePlaceholderRegex();

    [GeneratedRegex(@"\{\{\s*site\.name\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex SiteNamePlaceholderRegex();

    [GeneratedRegex(@"\{\{\s*site\.description\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex SiteDescriptionPlaceholderRegex();

    [GeneratedRegex(@"\{\{\s*year\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex YearPlaceholderRegex();

    [GeneratedRegex(@"\{\{\s*epoch\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex EpochPlaceholderRegex();

    [GeneratedRegex(@"\{\{\s*permalink\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex PermalinkPlaceholderRegex();

    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        // Use regex to replace placeholders with optional spaces
        // Matches {{placeholder}} or {{ placeholder }} or {{  placeholder  }} etc.
        var result = ContentPlaceholderRegex().Replace(context.Content, context.PageBody);
        result = TitlePlaceholderRegex().Replace(result, context.PageTitle);
        result = SiteNamePlaceholderRegex().Replace(result, context.SiteConfig.Name);
        result = SiteDescriptionPlaceholderRegex().Replace(result, context.SiteConfig.Description);
        result = YearPlaceholderRegex().Replace(result, context.CurrentYear);
        result = EpochPlaceholderRegex().Replace(result, context.CurrentEpoch);
        result = PermalinkPlaceholderRegex().Replace(result, context.Permalink);
        
        context.Content = result;
        return Task.FromResult(context);
    }
}
