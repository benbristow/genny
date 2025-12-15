using Genny.Constants;

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

        var pagesDirectory = Path.Combine(context.RootDirectory, DirectoryConstants.Pages);
        context.Permalink = PageUrlHelper.CalculatePageUrl(context.FilePath, pagesDirectory, context.SiteConfig.BaseUrl);
        
        Logger.LogVerbose($"      Permalink: {context.Permalink}", context.Verbose);
        
        return Task.FromResult(context);
    }
}
