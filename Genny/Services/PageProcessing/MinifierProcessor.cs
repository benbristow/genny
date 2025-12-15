using WebMarkupMin.Core;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that minifies HTML content using WebMarkupMin.
/// </summary>
public class MinifierProcessor : IPageProcessor
{
    private static readonly HtmlMinifier Minifier = new HtmlMinifier();

    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Content))
        {
            return Task.FromResult(context);
        }

        var result = Minifier.Minify(context.Content);
        
        // Use minified content if available, otherwise fall back to original
        // WebMarkupMin may produce warnings but still provide minified content
        if (!string.IsNullOrEmpty(result.MinifiedContent))
        {
            context.Content = result.MinifiedContent;
        }
        // If minification completely fails, keep original content
        
        return Task.FromResult(context);
    }
}
