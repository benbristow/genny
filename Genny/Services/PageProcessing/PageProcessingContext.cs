using Genny.Models;

namespace Genny.Services.PageProcessing;

/// <summary>
/// Context object that holds all data needed for page processing.
/// </summary>
public class PageProcessingContext
{
    public string Content { get; set; } = string.Empty;
    public string PageBody { get; set; } = string.Empty;
    public string PageTitle { get; set; } = string.Empty;
    public SiteConfig SiteConfig { get; set; } = null!;
    public string CurrentYear { get; set; } = string.Empty;
    public string CurrentEpoch { get; set; } = string.Empty;
    public string Permalink { get; set; } = string.Empty;
    public string RootDirectory { get; set; } = string.Empty;
    public HashSet<string> IncludedPartials { get; set; } = new();
    
    // Additional properties for page processing
    public string FilePath { get; set; } = string.Empty;
    public string OriginalPageContent { get; set; } = string.Empty;
    public string LayoutName { get; set; } = string.Empty;
}
