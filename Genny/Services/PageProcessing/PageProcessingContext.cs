using Genny.Models;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Represents all contextual information used and modified during
/// the processing lifecycle of a page. Properties of this context
/// may be produced, mutated, or consumed by various page processors
/// to build the final output.
/// </summary>
public class PageProcessingContext
{
    /// <summary>
    /// The current form of the page's content being processed.
    /// This may be mutated at each processor stage.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The "body" portion of the page's content, typically representing main content after comment removal.
    /// </summary>
    public string PageBody { get; set; } = string.Empty;

    /// <summary>
    /// The extracted (or default) title of the page.
    /// </summary>
    public string PageTitle { get; set; } = string.Empty;

    /// <summary>
    /// The active site configuration for use in placeholder replacements and page logic.
    /// </summary>
    public SiteConfig SiteConfig { get; set; } = null!;

    /// <summary>
    /// The current year as a string, used for {{ year }} replacements.
    /// </summary>
    public string CurrentYear { get; set; } = string.Empty;

    /// <summary>
    /// The current UNIX epoch timestamp string, used for {{ epoch }} replacements.
    /// </summary>
    public string CurrentEpoch { get; set; } = string.Empty;

    /// <summary>
    /// The calculated permalink (URL) for the current page.
    /// </summary>
    public string Permalink { get; set; } = string.Empty;

    /// <summary>
    /// The root directory path for the site, used as a base for resolving files such as partials and layouts.
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Set of full paths for partials that have already been included, to prevent circular references.
    /// </summary>
    public HashSet<string> IncludedPartials { get; set; } = new();

    /// <summary>
    /// The file path of the source page being processed.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// The original, unprocessed page content (read from file).
    /// Used for processors that need raw, unmodified data.
    /// </summary>
    public string OriginalPageContent { get; set; } = string.Empty;

    /// <summary>
    /// The extracted layout name to be applied to the page (e.g., "default.html").
    /// </summary>
    public string LayoutName { get; set; } = string.Empty;
}
