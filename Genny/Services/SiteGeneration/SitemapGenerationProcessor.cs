namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that generates sitemap.xml if enabled in configuration.
/// </summary>
public class SitemapGenerationProcessor : ISiteProcessor
{
    public async Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        if (context.SiteConfig.GenerateSitemap)
        {
            Logger.LogVerbose("Generating sitemap.xml...", context.Verbose);
            
            var sitemapContent = await SitemapGenerator.GenerateSitemapAsync(
                context.Pages, 
                context.PagesDirectory, 
                context.SiteConfig.BaseUrl);
                
            if (sitemapContent != null)
            {
                var sitemapPath = Path.Combine(context.SiteConfig.OutputDirectory, "sitemap.xml");
                await File.WriteAllTextAsync(sitemapPath, sitemapContent);
                
                Logger.LogVerbose("  Created: sitemap.xml", context.Verbose);
            }
        }

        return context;
    }
}
