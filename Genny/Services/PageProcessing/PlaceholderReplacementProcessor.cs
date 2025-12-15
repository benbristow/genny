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
        var result = context.Content;
        var replacements = 0;
        
        var before = result;
        result = ContentPlaceholderRegex().Replace(result, context.PageBody);
        if (result != before) replacements += ContentPlaceholderRegex().Matches(before).Count;
        
        before = result;
        result = TitlePlaceholderRegex().Replace(result, context.PageTitle);
        if (result != before) replacements += TitlePlaceholderRegex().Matches(before).Count;
        
        before = result;
        result = SiteNamePlaceholderRegex().Replace(result, context.SiteConfig.Name);
        if (result != before) replacements += SiteNamePlaceholderRegex().Matches(before).Count;
        
        before = result;
        result = SiteDescriptionPlaceholderRegex().Replace(result, context.SiteConfig.Description);
        if (result != before) replacements += SiteDescriptionPlaceholderRegex().Matches(before).Count;
        
        before = result;
        result = YearPlaceholderRegex().Replace(result, context.CurrentYear);
        if (result != before) replacements += YearPlaceholderRegex().Matches(before).Count;
        
        before = result;
        result = EpochPlaceholderRegex().Replace(result, context.CurrentEpoch);
        if (result != before) replacements += EpochPlaceholderRegex().Matches(before).Count;
        
        before = result;
        result = PermalinkPlaceholderRegex().Replace(result, context.Permalink);
        if (result != before) replacements += PermalinkPlaceholderRegex().Matches(before).Count;
        
        if (context.Verbose && replacements > 0)
        {
            Logger.LogVerbose($"      Replaced {replacements} placeholder(s)", context.Verbose);
        }
        
        context.Content = result;
        return Task.FromResult(context);
    }
}
