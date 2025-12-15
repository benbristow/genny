using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Services;

public static class PageBuilder
{
    public static async Task<string> BuildPageAsync(string filePath, string rootDirectory, SiteConfig siteConfig, bool verbose = false)
    {
        // Read page content
        var pageContent = await File.ReadAllTextAsync(filePath);
        
        Logger.LogVerbose($"  Processing: {Path.GetRelativePath(rootDirectory, filePath)}", verbose);
        
        // Create initial context
        var context = new PageProcessingContext
        {
            Content = pageContent,
            OriginalPageContent = pageContent,
            FilePath = filePath,
            RootDirectory = rootDirectory,
            SiteConfig = siteConfig,
            IncludedPartials = [],
            Verbose = verbose
        };
        
        // Build and execute processing pipeline
        var pipeline = CreateProcessingPipeline(siteConfig);
        context = await pipeline.ProcessAsync(context);
        
        if (verbose)
        {
            var sizeBefore = pageContent.Length;
            var sizeAfter = context.Content.Length;
            var change = sizeAfter > sizeBefore ? "+" : "";
            Logger.LogVerbose($"    Size: {sizeBefore} -> {sizeAfter} bytes ({change}{sizeAfter - sizeBefore})", verbose);
        }
        
        return context.Content;
    }

    private static PageProcessingPipeline CreateProcessingPipeline(SiteConfig siteConfig)
    {
        var pipeline = new PageProcessingPipeline()
            .AddProcessor(new MetadataExtractionProcessor())
            .AddProcessor(new PermalinkCalculationProcessor())
            .AddProcessor(new CommentRemovalProcessor())
            .AddProcessor(new LayoutApplicationProcessor())
            .AddProcessor(new PlaceholderReplacementProcessor())
            .AddProcessor(new PartialInclusionProcessor());

        // Conditionally add minifier based on config
        if (siteConfig.MinifyOutput)
        {
            pipeline.AddProcessor(new MinifierProcessor());
        }

        return pipeline;
    }
}