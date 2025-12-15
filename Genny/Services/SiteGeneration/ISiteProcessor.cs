namespace Genny.Services.SiteGeneration;

/// <summary>
/// Interface for site generation steps in the processing chain.
/// </summary>
public interface ISiteProcessor
{
    /// <summary>
    /// Processes the context and returns the mutated context.
    /// </summary>
    Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context);
}
