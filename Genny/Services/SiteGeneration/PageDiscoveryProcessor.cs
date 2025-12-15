namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that discovers all HTML pages in the pages directory.
/// </summary>
public class PageDiscoveryProcessor : ISiteProcessor
{
    public Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        var pagesDirectory = Path.Combine(context.SiteConfig.RootDirectory, "pages");
        context.PagesDirectory = pagesDirectory;
        context.Pages = GetPageTreeRecursive(pagesDirectory);

        Logger.Log($"Found {context.Pages.Count} page(s)");
        Logger.LogEmptyVerbose(context.Verbose);

        return Task.FromResult(context);
    }

    private static List<string> GetPageTreeRecursive(string directory)
    {
        var pages = new List<string>();
        
        if (!Directory.Exists(directory))
        {
            return pages;
        }

        // Get all HTML files in current directory
        pages.AddRange(Directory.GetFiles(directory, "*.html"));

        // Recursively get files from subdirectories (ignore build directory and other ignored directories)
        foreach (var subDirectory in Directory.GetDirectories(directory))
        {
            var directoryName = Path.GetFileName(subDirectory);
            if (IgnoredFilesConstants.IgnoredDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var subPages = GetPageTreeRecursive(subDirectory);
            pages.AddRange(subPages);
        }

        return pages;
    }
}
