namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that extracts metadata (year, epoch) for placeholder replacement.
/// </summary>
public class MetadataExtractionProcessor : IPageProcessor
{
    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        context.CurrentYear = DateTime.Now.Year.ToString();
        context.CurrentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        return Task.FromResult(context);
    }
}
