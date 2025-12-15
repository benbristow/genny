namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that logs configuration information when verbose mode is enabled.
/// </summary>
public class ConfigurationLoggingProcessor : ISiteProcessor
{
    public Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        if (context.Verbose)
        {
            Logger.Log("Configuration:");
            Logger.Log($"  Root directory: {context.SiteConfig.RootDirectory}");
            Logger.Log($"  Output directory: {context.SiteConfig.OutputDirectory}");
            Logger.Log($"  Site name: {context.SiteConfig.Name}");
            Logger.Log($"  Minify output: {context.SiteConfig.MinifyOutput}");
            Logger.Log($"  Generate sitemap: {context.SiteConfig.GenerateSitemap}");
            Logger.LogEmpty();
        }

        return Task.FromResult(context);
    }
}
