using System.Text.RegularExpressions;
using Genny.Models;

namespace Genny.Services;

public static partial class PageBuilder
{
    private const string DefaultLayoutName = "default.html";

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

    [GeneratedRegex(@"<!--\s*layout:\s*([^\s]+)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex LayoutCommentRegex();

    [GeneratedRegex(@"<!--\s*title:\s*(.+?)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex TitleCommentRegex();

    [GeneratedRegex(@"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TitleTagRegex();

    [GeneratedRegex(@"<body[^>]*>(.*?)</body>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BodyTagRegex();

    public static async Task<string> BuildPageAsync(string filePath, string rootDirectory, SiteConfig siteConfig)
    {
        var pageContent = await File.ReadAllTextAsync(filePath);
        
        // Extract page title
        var pageTitle = ExtractPageTitle(pageContent);
        
        // Get current year and epoch for placeholder replacement
        var currentYear = DateTime.Now.Year.ToString();
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        // Check if page specifies a layout
        var layoutName = ExtractLayoutName(pageContent);
        if (string.IsNullOrEmpty(layoutName))
        {
            layoutName = DefaultLayoutName;
        }

        // Try to load layout
        var layoutPath = Path.Combine(rootDirectory, "layouts", layoutName);
        if (File.Exists(layoutPath))
        {
            var layoutContent = await File.ReadAllTextAsync(layoutPath);
            // Remove comments before extracting body
            var cleanedContent = RemoveComments(pageContent);
            var pageBody = ExtractPageBody(cleanedContent);
            
            // Replace placeholders (handles spaces in placeholders)
            var result = ReplacePlaceholders(layoutContent, pageBody, pageTitle, siteConfig, currentYear, currentEpoch);
            
            return result;
        }

        // No layout found, remove comments and replace placeholders in page content
        var cleanedPageContent = RemoveComments(pageContent);
        return ReplacePlaceholders(cleanedPageContent, string.Empty, pageTitle, siteConfig, currentYear, currentEpoch);
    }

    private static string ReplacePlaceholders(string content, string pageBody, string pageTitle, SiteConfig siteConfig, string currentYear, string currentEpoch)
    {
        // Use regex to replace placeholders with optional spaces
        // Matches {{placeholder}} or {{ placeholder }} or {{  placeholder  }} etc.
        var result = ContentPlaceholderRegex().Replace(content, pageBody);
        result = TitlePlaceholderRegex().Replace(result, pageTitle);
        result = SiteNamePlaceholderRegex().Replace(result, siteConfig.Name);
        result = SiteDescriptionPlaceholderRegex().Replace(result, siteConfig.Description);
        result = YearPlaceholderRegex().Replace(result, currentYear);
        result = EpochPlaceholderRegex().Replace(result, currentEpoch);
        
        return result;
    }

    private static string ExtractLayoutName(string pageContent)
    {
        // Look for <!-- layout: layoutname.html --> or similar
        var layoutMatch = LayoutCommentRegex().Match(pageContent);

        if (layoutMatch is { Success: true, Groups.Count: > 1 })
        {
            return layoutMatch.Groups[1].Value.Trim();
        }

        return string.Empty;
    }

    private static string ExtractPageTitle(string pageContent)
    {
        // First, try to extract from <!-- title: Page Title --> comment
        var titleCommentMatch = TitleCommentRegex().Match(pageContent);

        if (titleCommentMatch is { Success: true, Groups.Count: > 1 })
        {
            return titleCommentMatch.Groups[1].Value.Trim();
        }

        // Second, try to extract from <title> tag
        var titleTagMatch = TitleTagRegex().Match(pageContent);

        if (titleTagMatch is { Success: true, Groups.Count: > 1 })
        {
            return titleTagMatch.Groups[1].Value.Trim();
        }

        // No title found, return empty string
        return string.Empty;
    }

    private static string ExtractPageBody(string pageContent)
    {
        // Extract content between <body> tags, or return full content if no body tag
        var bodyMatch = BodyTagRegex().Match(pageContent);

        if (bodyMatch is { Success: true, Groups.Count: > 1 })
        {
            return bodyMatch.Groups[1].Value.Trim();
        }

        // If no body tag, return the full content
        return pageContent;
    }

    [GeneratedRegex(@"\s*<!--\s*layout:\s*[^\s]+\s*-->\s*", RegexOptions.IgnoreCase)]
    private static partial Regex LayoutCommentRemovalRegex();

    [GeneratedRegex(@"\s*<!--\s*title:\s*.+?\s*-->\s*", RegexOptions.IgnoreCase)]
    private static partial Regex TitleCommentRemovalRegex();

    private static string RemoveComments(string pageContent)
    {
        // Remove layout comments: <!-- layout: layoutname.html --> (including surrounding whitespace)
        var result = LayoutCommentRemovalRegex().Replace(pageContent, string.Empty);

        // Remove title comments: <!-- title: Page Title --> (including surrounding whitespace)
        result = TitleCommentRemovalRegex().Replace(result, string.Empty);

        return result;
    }
}