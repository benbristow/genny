namespace Genny.Services.PageProcessing;

/// <summary>
/// Pipeline that chains multiple page processors together.
/// </summary>
public class PageProcessingPipeline
{
    private readonly List<IPageProcessor> _processors = new();

    /// <summary>
    /// Adds a processor to the pipeline.
    /// </summary>
    public PageProcessingPipeline AddProcessor(IPageProcessor processor)
    {
        _processors.Add(processor);
        return this;
    }

    /// <summary>
    /// Processes the context through all processors in sequence.
    /// </summary>
    public async Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        foreach (var processor in _processors)
        {
            context = await processor.ProcessAsync(context);
        }
        
        return context;
    }
}
