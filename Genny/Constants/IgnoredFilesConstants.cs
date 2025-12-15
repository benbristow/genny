namespace Genny.Constants;

/// <summary>
/// Constants for files and directories that should be ignored during site generation.
/// </summary>
public static class IgnoredFilesConstants
{
    /// <summary>
    /// Files that should be ignored when copying public assets.
    /// </summary>
    public static readonly string[] IgnoredFiles = 
    {
        ".gitignore",
        ".env",
        ".env.local",
        ".env.production",
        "package.json",
        "package-lock.json",
        "yarn.lock",
        "pnpm-lock.yaml",
        ".git",
        ".gitattributes",
        ".gitkeep",
        ".DS_Store",
        "Thumbs.db"
    };
    
    /// <summary>
    /// Directories that should be ignored during page discovery and asset copying.
    /// </summary>
    public static readonly string[] IgnoredDirectories = 
    {
        "node_modules",
        ".git",
        ".vscode",
        ".idea",
        ".vs",
        ".next",
        ".nuxt",
        "dist",
        DirectoryConstants.Build,
        ".cache",
        DirectoryConstants.Layouts
    };
}
