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

    [GeneratedRegex(@"\{\{\s*permalink\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex PermalinkPlaceholderRegex();

    [GeneratedRegex(@"\{\{\s*partial\s*:\s*([^\s}]+)\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex PartialPlaceholderRegex();

    [GeneratedRegex(@"<!--\s*layout:\s*([^\s]+)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex LayoutCommentRegex();

    [GeneratedRegex(@"<!--\s*title:\s*(.+?)\s*-->", RegexOptions.IgnoreCase)]
    private static partial Regex TitleCommentRegex();

    [GeneratedRegex(@"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TitleTagRegex();

    [GeneratedRegex(@"\s*<!--\s*layout:\s*[^\s]+\s*-->\s*", RegexOptions.IgnoreCase)]
    private static partial Regex LayoutCommentRemovalRegex();

    [GeneratedRegex(@"\s*<!--\s*title:\s*.+?\s*-->\s*", RegexOptions.IgnoreCase)]
    private static partial Regex TitleCommentRemovalRegex();

    public static async Task<string> BuildPageAsync(string filePath, string rootDirectory, SiteConfig siteConfig)
    {
        var pageContent = await File.ReadAllTextAsync(filePath);
        
        // Extract page title
        var pageTitle = ExtractPageTitle(pageContent);
        
        // Get current year and epoch for placeholder replacement
        var currentYear = DateTime.Now.Year.ToString();
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        // Calculate permalink
        var permalink = CalculatePermalink(filePath, rootDirectory, siteConfig);
        
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
            var pageBody = RemoveComments(pageContent);
            
        // Replace placeholders (handles spaces in placeholders)
        var result = await ReplacePlaceholdersAsync(layoutContent, pageBody, pageTitle, siteConfig, currentYear, currentEpoch, permalink, rootDirectory, new HashSet<string>());
        
        return result;
        }

        // No layout found, remove comments and replace placeholders in page content
        var cleanedPageContent = RemoveComments(pageContent);
        return await ReplacePlaceholdersAsync(cleanedPageContent, string.Empty, pageTitle, siteConfig, currentYear, currentEpoch, permalink, rootDirectory, new HashSet<string>());
    }

    private static async Task<string> ReplacePlaceholdersAsync(string content, string pageBody, string pageTitle, SiteConfig siteConfig, string currentYear, string currentEpoch, string permalink, string rootDirectory, HashSet<string> includedPartials)
    {
        // Replace placeholders (handles spaces in placeholders)
        var result = ReplacePlaceholderValues(content, pageBody, pageTitle, siteConfig, currentYear, currentEpoch, permalink);
        
        // Process partials (recursive, with circular reference prevention)
        result = await ProcessPartialsAsync(result, rootDirectory, includedPartials, siteConfig, currentYear, currentEpoch, permalink, pageTitle);
        
        return result;
    }

    private static string ReplacePlaceholderValues(string content, string pageBody, string pageTitle, SiteConfig siteConfig, string currentYear, string currentEpoch, string permalink)
    {
        // Use regex to replace placeholders with optional spaces
        // Matches {{placeholder}} or {{ placeholder }} or {{  placeholder  }} etc.
        var result = ContentPlaceholderRegex().Replace(content, pageBody);
        result = TitlePlaceholderRegex().Replace(result, pageTitle);
        result = SiteNamePlaceholderRegex().Replace(result, siteConfig.Name);
        result = SiteDescriptionPlaceholderRegex().Replace(result, siteConfig.Description);
        result = YearPlaceholderRegex().Replace(result, currentYear);
        result = EpochPlaceholderRegex().Replace(result, currentEpoch);
        result = PermalinkPlaceholderRegex().Replace(result, permalink);
        
        return result;
    }

    private static async Task<string> ProcessPartialsAsync(string content, string rootDirectory, HashSet<string> includedPartials, SiteConfig siteConfig, string currentYear, string currentEpoch, string permalink, string pageTitle)
    {
        var result = content;
        var partialsDir = Path.Combine(rootDirectory, "partials");
        
        // Find all partial placeholders
        var matches = PartialPlaceholderRegex().Matches(result);
        
        // Process in reverse order to maintain string indices when replacing
        var matchesList = matches.Reverse().ToList();
        
        foreach (var match in matchesList)
        {
            if (match is not { Success: true, Groups.Count: > 1 }) continue;
            var partialName = match.Groups[1].Value.Trim();
            var partialPath = Path.Combine(partialsDir, partialName);
                
            // Check for circular reference
            if (includedPartials.Contains(partialPath))
            {
                // Circular reference detected - remove placeholder to prevent infinite loop
                result = result.Remove(match.Index, match.Length);
                continue;
            }
                
            // Check if partial exists
            if (Directory.Exists(partialsDir) && File.Exists(partialPath))
            {
                // Add to included set to prevent circular references
                includedPartials.Add(partialPath);
                    
                // Load partial content
                var partialContent = await File.ReadAllTextAsync(partialPath);
                
                // Replace placeholders in partial content (use empty string for content as it's page-specific)
                partialContent = ReplacePlaceholderValues(partialContent, string.Empty, pageTitle, siteConfig, currentYear, currentEpoch, permalink);
                    
                // Recursively process partials within the partial
                var processedPartial = await ProcessPartialsAsync(partialContent, rootDirectory, includedPartials, siteConfig, currentYear, currentEpoch, permalink, pageTitle);
                    
                // Replace the placeholder with the partial content
                result = result.Remove(match.Index, match.Length).Insert(match.Index, processedPartial);
                    
                // Remove from included set after processing (allows same partial to be included multiple times in different contexts)
                includedPartials.Remove(partialPath);
            }
            else
            {
                // Partial not found or directory doesn't exist - remove placeholder
                result = result.Remove(match.Index, match.Length);
            }
        }
        
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

    private static string RemoveComments(string pageContent)
    {
        // Remove layout comments: <!-- layout: layoutname.html --> (including surrounding whitespace)
        var result = LayoutCommentRemovalRegex().Replace(pageContent, string.Empty);

        // Remove title comments: <!-- title: Page Title --> (including surrounding whitespace)
        result = TitleCommentRemovalRegex().Replace(result, string.Empty);

        return result;
    }

    private static string CalculatePermalink(string filePath, string rootDirectory, SiteConfig siteConfig)
    {
        var pagesDirectory = Path.Combine(rootDirectory, "pages");
        return PageUrlHelper.CalculatePageUrl(filePath, pagesDirectory, siteConfig.BaseUrl);
    }
}