namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that creates the build directory.
/// </summary>
public class BuildDirectoryCreationProcessor : ISiteProcessor
{
    public Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        Directory.CreateDirectory(context.SiteConfig.OutputDirectory);
        return Task.FromResult(context);
    }
}
