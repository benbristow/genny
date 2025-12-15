using Genny.Models;

namespace Genny.Services.SiteGeneration;

/// <summary>
/// Represents all contextual information used and modified during
/// the site generation lifecycle. Properties of this context
/// may be produced, mutated, or consumed by various site processors
/// to build the final site.
/// </summary>
public class SiteGenerationContext
{
    /// <summary>
    /// The active site configuration.
    /// </summary>
    public SiteConfig SiteConfig { get; set; } = null!;

    /// <summary>
    /// List of discovered page file paths.
    /// </summary>
    public List<string> Pages { get; set; } = new();

    /// <summary>
    /// The pages directory path.
    /// </summary>
    public string PagesDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Whether verbose logging is enabled.
    /// </summary>
    public bool Verbose { get; set; } = false;

    /// <summary>
    /// Number of files copied from public directory.
    /// </summary>
    public int CopiedPublicFiles { get; set; } = 0;

    /// <summary>
    /// Number of pages processed.
    /// </summary>
    public int ProcessedPages { get; set; } = 0;
}
