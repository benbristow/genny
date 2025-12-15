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

        var originalSize = context.Content.Length;
        var result = Minifier.Minify(context.Content);
        
        // Use minified content if available, otherwise fall back to original
        // WebMarkupMin may produce warnings but still provide minified content
        if (!string.IsNullOrEmpty(result.MinifiedContent))
        {
            context.Content = result.MinifiedContent;
            
            if (context.Verbose)
            {
                var newSize = result.MinifiedContent.Length;
                var reduction = originalSize - newSize;
                var percent = originalSize > 0 ? (reduction * 100.0 / originalSize) : 0;
                Console.WriteLine($"      Minified: {originalSize} -> {newSize} bytes ({percent:F1}% reduction)");
                
                if (result.Errors.Count > 0)
                {
                    Console.WriteLine($"      Minification warnings: {result.Errors.Count}");
                }
            }
        }
        else if (context.Verbose)
        {
            Console.WriteLine($"      Minification failed, using original content");
        }
        // If minification completely fails, keep original content
        
        return Task.FromResult(context);
    }
}
