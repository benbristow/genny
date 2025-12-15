namespace Genny.Services;

/// <summary>
/// Helper class for calculating page URLs and permalinks.
/// </summary>
public static class PageUrlHelper
{
    /// <summary>
    /// Calculates the permalink/URL for a page file path.
    /// For root pages, flattens to just filename (e.g., pages/index.html -> index.html or /).
    /// For subdirectory pages, preserves structure (e.g., pages/about/index.html -> about/index.html).
    /// </summary>
    /// <param name="pagePath">Full path to the page file</param>
    /// <param name="pagesDirectory">Full path to the pages directory</param>
    /// <param name="baseUrl">Optional base URL (e.g., "https://example.com")</param>
    /// <returns>The full URL for the page</returns>
    public static string CalculatePageUrl(string pagePath, string pagesDirectory, string? baseUrl = null)
    {
        var relativePath = Path.GetRelativePath(pagesDirectory, pagePath);
        
        // For root pages, flatten to just filename (e.g., pages/index.html -> index.html)
        // For subdirectory pages, preserve structure (e.g., pages/about/index.html -> about/index.html)
        string urlPath;
        if (Path.GetDirectoryName(relativePath) == "." || string.IsNullOrEmpty(Path.GetDirectoryName(relativePath)))
        {
            var fileName = Path.GetFileName(pagePath);
            urlPath = fileName == "index.html" ? "" : fileName;
        }
        else
        {
            urlPath = relativePath.Replace('\\', '/');
        }

        // Build full URL
        string fullUrl;
        if (baseUrl != null)
        {
            fullUrl = urlPath == "" 
                ? baseUrl.TrimEnd('/')
                : $"{baseUrl.TrimEnd('/')}/{urlPath}";
        }
        else
        {
            fullUrl = urlPath == "" ? "/" : $"/{urlPath}";
        }

        return fullUrl;
    }
}
