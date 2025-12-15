namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that processes and writes all discovered pages to the build directory.
/// </summary>
public class PageProcessingProcessor : ISiteProcessor
{
    public async Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        foreach (var pagePath in context.Pages)
        {
            // Get relative path from pages directory
            var relativePath = Path.GetRelativePath(context.PagesDirectory, pagePath);
            
            // For root pages, flatten to just filename (e.g., pages/index.html -> index.html)
            // For subdirectory pages, preserve structure (e.g., pages/about/index.html -> about/index.html)
            string destinationPath;
            if (Path.GetDirectoryName(relativePath) == "." || string.IsNullOrEmpty(Path.GetDirectoryName(relativePath)))
            {
                // Root level page - flatten to filename
                var fileName = Path.GetFileName(pagePath);
                destinationPath = Path.Combine(context.SiteConfig.OutputDirectory, fileName);
            }
            else
            {
                // Subdirectory page - preserve relative path structure
                destinationPath = Path.Combine(context.SiteConfig.OutputDirectory, relativePath);
            }
            
            // Ensure destination directory exists
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            
            Logger.LogVerbose($"Copying {relativePath} -> {Path.GetRelativePath(context.SiteConfig.OutputDirectory, destinationPath)}", context.Verbose);
            
            // Build the page and copy to build directory
            var html = await PageBuilder.BuildPageAsync(pagePath, context.SiteConfig.RootDirectory, context.SiteConfig, context.Verbose);
            await File.WriteAllTextAsync(destinationPath, html);
            
            Logger.LogVerbose($"    Written: {Path.GetRelativePath(context.SiteConfig.OutputDirectory, destinationPath)}", context.Verbose);
            
            context.ProcessedPages++;
        }

        return context;
    }
}
