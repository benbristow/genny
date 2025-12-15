// Represents the configuration settings for a Genny static site.
namespace Genny.Models;

/// <summary>
/// Configuration settings loaded from genny.toml and inferred site structure.
/// </summary>
public class SiteConfig
{
    /// <summary>
    /// The name of the site (from config).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the site (from config).
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The root directory of the site (inferred from config file path).
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    /// The output/build directory for generated files.
    /// </summary>
    public string OutputDirectory { get; set; } = string.Empty;

    /// <summary>
    /// The base URL for the site, used in sitemaps and absolute URLs (optional).
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Whether to generate a sitemap.xml file (defaults to true).
    /// </summary>
    public bool GenerateSitemap { get; set; } = true;

    /// <summary>
    /// Whether to minify HTML output by removing unnecessary whitespace (defaults to true).
    /// </summary>
    public bool MinifyOutput { get; set; } = true;
}
