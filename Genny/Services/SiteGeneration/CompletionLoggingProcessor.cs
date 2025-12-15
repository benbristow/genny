namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that logs completion message.
/// </summary>
public class CompletionLoggingProcessor : ISiteProcessor
{
    public Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        Logger.LogEmptyVerbose(context.Verbose);
        Logger.Log($"Site generated to {context.SiteConfig.OutputDirectory}");

        return Task.FromResult(context);
    }
}
