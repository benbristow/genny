using System.Text.RegularExpressions;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that includes partial files in content, with circular reference prevention.
/// </summary>
public partial class PartialInclusionProcessor : IPageProcessor
{
    [GeneratedRegex(@"\{\{\s*partial\s*:\s*([^\s}]+)\s*\}\}", RegexOptions.IgnoreCase)]
    private static partial Regex PartialPlaceholderRegex();

    private readonly PlaceholderReplacementProcessor _placeholderProcessor = new();

    public async Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        var result = context.Content;
        var partialsDir = Path.Combine(context.RootDirectory, "partials");
        
        // Find all partial placeholders
        var matches = PartialPlaceholderRegex().Matches(result);
        
        // Process in reverse order to maintain string indices when replacing
        var matchesList = matches.Reverse().ToList();
        
        var includedCount = 0;
        foreach (var match in matchesList)
        {
            if (match is not { Success: true, Groups.Count: > 1 }) continue;
            var partialName = match.Groups[1].Value.Trim();
            var partialPath = Path.Combine(partialsDir, partialName);
                
            // Check for circular reference
            if (context.IncludedPartials.Contains(partialPath))
            {
                // Circular reference detected - remove placeholder to prevent infinite loop
                if (context.Verbose)
                {
                    Console.WriteLine($"      Skipping circular reference: {partialName}");
                }
                result = result.Remove(match.Index, match.Length);
                continue;
            }
                
            // Check if partial exists
            if (Directory.Exists(partialsDir) && File.Exists(partialPath))
            {
                // Add to included set to prevent circular references
                context.IncludedPartials.Add(partialPath);
                    
                // Load partial content
                var partialContent = await File.ReadAllTextAsync(partialPath);
                
                if (context.Verbose)
                {
                    Console.WriteLine($"      Including partial: {partialName}");
                }
                
                // Create context for partial processing
                var partialContext = new PageProcessingContext
                {
                    Content = partialContent,
                    PageBody = string.Empty, // Partials don't have page body
                    PageTitle = context.PageTitle,
                    SiteConfig = context.SiteConfig,
                    CurrentYear = context.CurrentYear,
                    CurrentEpoch = context.CurrentEpoch,
                    Permalink = context.Permalink,
                    RootDirectory = context.RootDirectory,
                    IncludedPartials = context.IncludedPartials,
                    Verbose = context.Verbose
                };
                
                // Replace placeholders in partial content
                partialContext = await _placeholderProcessor.ProcessAsync(partialContext);
                    
                // Recursively process partials within the partial
                partialContext = await ProcessAsync(partialContext);
                    
                // Replace the placeholder with the partial content
                result = result.Remove(match.Index, match.Length).Insert(match.Index, partialContext.Content);
                    
                // Remove from included set after processing (allows same partial to be included multiple times in different contexts)
                context.IncludedPartials.Remove(partialPath);
                includedCount++;
            }
            else
            {
                // Partial not found or directory doesn't exist - remove placeholder
                if (context.Verbose)
                {
                    Console.WriteLine($"      Partial not found: {partialName}");
                }
                result = result.Remove(match.Index, match.Length);
            }
        }
        
        if (context.Verbose && includedCount > 0)
        {
            Console.WriteLine($"      Included {includedCount} partial(s)");
        }
        
        context.Content = result;
        return context;
    }
}
