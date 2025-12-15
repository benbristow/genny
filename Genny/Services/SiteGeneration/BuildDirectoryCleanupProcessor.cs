namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that cleans the build directory if it exists.
/// </summary>
public class BuildDirectoryCleanupProcessor : ISiteProcessor
{
    public Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        if (Directory.Exists(context.SiteConfig.OutputDirectory))
        {
            Logger.LogVerbose("Cleaning build directory...", context.Verbose);
            Directory.Delete(context.SiteConfig.OutputDirectory, recursive: true);
        }

        return Task.FromResult(context);
    }
}
