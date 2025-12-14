using Genny.Models;

namespace Genny.Services;

public static class SiteGenerator
{
    private const string BuildDirectory = "build";
    
    private static readonly string[] IgnoredFiles = 
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
    
    private static readonly string[] IgnoredDirectories = 
    {
        "node_modules",
        ".git",
        ".vscode",
        ".idea",
        ".vs",
        ".next",
        ".nuxt",
        "dist",
        BuildDirectory,
        ".cache",
        "layouts"
    };

    public static async Task GenerateSiteAsync(SiteConfig siteConfig, bool verbose = false)
    {
        var pages = GetPageTree(siteConfig.RootDirectory);
        var pagesDirectory = Path.Combine(siteConfig.RootDirectory, "pages");
        
        if (verbose)
        {
            Console.WriteLine($"Root directory: {siteConfig.RootDirectory}");
            Console.WriteLine($"Output directory: {siteConfig.OutputDirectory}");
        }
        
        // Clean out build directory first
        if (Directory.Exists(siteConfig.OutputDirectory))
        {
            if (verbose)
            {
                Console.WriteLine("Cleaning build directory...");
            }
            Directory.Delete(siteConfig.OutputDirectory, recursive: true);
        }
        
        // Create fresh build directory
        Directory.CreateDirectory(siteConfig.OutputDirectory);
        
        // Copy public folder contents if it exists
        var publicDirectory = Path.Combine(siteConfig.RootDirectory, "public");
        if (Directory.Exists(publicDirectory))
        {
            CopyDirectoryContents(publicDirectory, siteConfig.OutputDirectory);
            if (verbose)
            {
                Console.WriteLine("Copied public folder contents to build directory");
            }
        }
        
        Console.WriteLine($"Found {pages.Count} pages");
        
        foreach (var pagePath in pages)
        {
            // Get relative path from pages directory
            var relativePath = Path.GetRelativePath(pagesDirectory, pagePath);
            
            // For root pages, flatten to just filename (e.g., pages/index.html -> index.html)
            // For subdirectory pages, preserve structure (e.g., pages/about/index.html -> about/index.html)
            string destinationPath;
            if (Path.GetDirectoryName(relativePath) == "." || string.IsNullOrEmpty(Path.GetDirectoryName(relativePath)))
            {
                // Root level page - flatten to filename
                var fileName = Path.GetFileName(pagePath);
                destinationPath = Path.Combine(siteConfig.OutputDirectory, fileName);
            }
            else
            {
                // Subdirectory page - preserve relative path structure
                destinationPath = Path.Combine(siteConfig.OutputDirectory, relativePath);
            }
            
            // Ensure destination directory exists
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            
            if (verbose)
            {
                Console.WriteLine($"Copying {relativePath} -> {Path.GetRelativePath(siteConfig.OutputDirectory, destinationPath)}");
            }
            
            // Build the page and copy to build directory
            var html = await PageBuilder.BuildPageAsync(pagePath, siteConfig.RootDirectory, siteConfig);
            await File.WriteAllTextAsync(destinationPath, html);
        }
        
        Console.WriteLine($"Site generated to {siteConfig.OutputDirectory}");
    }

    // Recursively get tree of html files in pages directory
    private static List<string> GetPageTree(string rootDirectory)
    {
        var pagesDirectory = Path.Combine(rootDirectory, "pages");
        return GetPageTreeRecursive(pagesDirectory);
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
            if (ShouldIgnoreDirectory(directoryName))
            {
                continue;
            }

            var subPages = GetPageTreeRecursive(subDirectory);
            pages.AddRange(subPages);
        }

        return pages;
    }

    private static void CopyDirectoryContents(string sourceDir, string destinationDir)
    {
        // Create destination directory if it doesn't exist
        Directory.CreateDirectory(destinationDir);

        // Copy all files (skip ignored files)
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            if (ShouldIgnoreFile(fileName))
            {
                continue;
            }
            
            var destFile = Path.Combine(destinationDir, fileName);
            File.Copy(file, destFile, overwrite: true);
        }

        // Recursively copy subdirectories (skip ignored directories)
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(subDir);
            if (ShouldIgnoreDirectory(dirName))
            {
                continue;
            }
            
            var destSubDir = Path.Combine(destinationDir, dirName);
            CopyDirectoryContents(subDir, destSubDir);
        }
    }
    
    private static bool ShouldIgnoreFile(string fileName)
    {
        return IgnoredFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase);
    }
    
    private static bool ShouldIgnoreDirectory(string directoryName)
    {
        return IgnoredDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
    }
}