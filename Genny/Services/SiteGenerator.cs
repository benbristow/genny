using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Services;

public static class SiteGenerator
{
    public static async Task GenerateSiteAsync(SiteConfig siteConfig, bool verbose = false)
    {
        // Create initial context
        var context = new SiteGenerationContext
        {
            SiteConfig = siteConfig,
            Verbose = verbose
        };

        // Build and execute processing pipeline
        var pipeline = CreateProcessingPipeline();
        await pipeline.ProcessAsync(context);
    }

    private static SiteGenerationPipeline CreateProcessingPipeline()
    {
        return new SiteGenerationPipeline()
            .AddProcessor(new ConfigurationLoggingProcessor())
            .AddProcessor(new BuildDirectoryCleanupProcessor())
            .AddProcessor(new BuildDirectoryCreationProcessor())
            .AddProcessor(new PageDiscoveryProcessor())
            .AddProcessor(new PublicAssetsCopyProcessor())
            .AddProcessor(new PageProcessingProcessor())
            .AddProcessor(new SitemapGenerationProcessor())
            .AddProcessor(new CompletionLoggingProcessor());
    }
}