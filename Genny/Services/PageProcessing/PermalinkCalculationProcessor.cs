namespace Genny.Services.PageProcessing;

/// <summary>
/// Processor that calculates the permalink for the page.
/// </summary>
public class PermalinkCalculationProcessor : IPageProcessor
{
    public Task<PageProcessingContext> ProcessAsync(PageProcessingContext context)
    {
        if (string.IsNullOrEmpty(context.FilePath))
        {
            return Task.FromResult(context);
        }

        var pagesDirectory = Path.Combine(context.RootDirectory, "pages");
        context.Permalink = PageUrlHelper.CalculatePageUrl(context.FilePath, pagesDirectory, context.SiteConfig.BaseUrl);
        
        return Task.FromResult(context);
    }
}
