using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Services;

public static class PageBuilder
{
    public static async Task<string> BuildPageAsync(string filePath, string rootDirectory, SiteConfig siteConfig)
    {
        // Read page content
        var pageContent = await File.ReadAllTextAsync(filePath);
        
        // Create initial context
        var context = new PageProcessingContext
        {
            Content = pageContent,
            OriginalPageContent = pageContent,
            FilePath = filePath,
            RootDirectory = rootDirectory,
            SiteConfig = siteConfig,
            IncludedPartials = []
        };
        
        // Build and execute processing pipeline
        var pipeline = CreateProcessingPipeline(siteConfig);
        context = await pipeline.ProcessAsync(context);
        
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