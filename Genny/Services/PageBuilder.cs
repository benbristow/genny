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
        var pipeline = CreateProcessingPipeline();
        context = await pipeline.ProcessAsync(context);
        
        return context.Content;
    }

    private static PageProcessingPipeline CreateProcessingPipeline()
    {
        return new PageProcessingPipeline()
            .AddProcessor(new MetadataExtractionProcessor())
            .AddProcessor(new PermalinkCalculationProcessor())
            .AddProcessor(new TitleExtractionProcessor())
            .AddProcessor(new LayoutExtractionProcessor())
            .AddProcessor(new CommentRemovalProcessor())
            .AddProcessor(new PageBodyExtractionProcessor())
            .AddProcessor(new LayoutApplicationProcessor())
            .AddProcessor(new PlaceholderReplacementProcessor())
            .AddProcessor(new PartialInclusionProcessor());
    }
}