namespace Genny.Services.PageProcessing;

/// <summary>
/// Interface for page processing steps in the processing chain.
/// </summary>
public interface IPageProcessor
{
    /// <summary>
    /// Processes the context and returns the mutated context.
    /// </summary>
    Task<PageProcessingContext> ProcessAsync(PageProcessingContext context);
}
