namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that extracts the page body from page content (after comments are removed).
/// </summary>
public class PageBodyExtractionProcessor : IPageProcessor
{
    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        // The page body is the content after comments are removed
        // This processor sets PageBody from Content
        context.PageBody = context.Content;
        
        return Task.FromResult(context);
    }
}
