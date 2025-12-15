namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that applies a layout to the page content.
/// </summary>
public class LayoutApplicationProcessor : IPageProcessor
{
    public async Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        var layoutPath = Path.Combine(context.RootDirectory, "layouts", context.LayoutName);
        
        if (File.Exists(layoutPath))
        {
            // Load layout content
            var layoutContent = await File.ReadAllTextAsync(layoutPath);
            
            // Set layout content as the main content, and page body as the body
            context.Content = layoutContent;
            // PageBody is already set from previous processing
            
            Logger.LogVerbose($"      Applied layout: {context.LayoutName}", context.Verbose);
        }
        else
        {
            Logger.LogVerbose($"      Layout not found: {context.LayoutName} (using page content as-is)", context.Verbose);
        }

        // If layout doesn't exist, content remains as-is (no layout applied)
        return context;
    }
}
