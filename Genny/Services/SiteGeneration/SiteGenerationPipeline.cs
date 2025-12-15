namespace Genny.Services.SiteGeneration;

/// <summary>
/// Pipeline that chains multiple site processors together.
/// </summary>
public class SiteGenerationPipeline
{
    private readonly List<ISiteProcessor> _processors = new();

    /// <summary>
    /// Adds a processor to the pipeline.
    /// </summary>
    public SiteGenerationPipeline AddProcessor(ISiteProcessor processor)
    {
        _processors.Add(processor);
        return this;
    }

    /// <summary>
    /// Processes the context through all processors in sequence.
    /// </summary>
    public async Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        foreach (var processor in _processors)
        {
            Logger.LogVerbose($"Running: {processor.GetType().Name}", context.Verbose);
            
            context = await processor.ProcessAsync(context);
        }
        
        return context;
    }
}
