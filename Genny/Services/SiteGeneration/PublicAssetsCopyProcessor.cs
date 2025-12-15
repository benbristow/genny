using Genny.Constants;

namespace Genny.Services.SiteGeneration;

/// <summary>
/// Processor that copies public assets to the build directory.
/// </summary>
public class PublicAssetsCopyProcessor : ISiteProcessor
{
    public Task<SiteGenerationContext> ProcessAsync(SiteGenerationContext context)
    {
        var publicDirectory = Path.Combine(context.SiteConfig.RootDirectory, DirectoryConstants.Public);
        
        if (Directory.Exists(publicDirectory))
        {
            Logger.LogVerbose("Copying public assets...", context.Verbose);
            
            var copiedFiles = CopyDirectoryContents(publicDirectory, context.SiteConfig.OutputDirectory, context.Verbose);
            context.CopiedPublicFiles = copiedFiles;
            
            Logger.LogVerbose($"  Copied {copiedFiles} file(s) from public directory", context.Verbose);
        }

        return Task.FromResult(context);
    }

    private static int CopyDirectoryContents(string sourceDir, string destinationDir, bool verbose = false)
    {
        var fileCount = 0;
        
        // Create destination directory if it doesn't exist
        Directory.CreateDirectory(destinationDir);

        // Copy all files (skip ignored files)
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            if (IgnoredFilesConstants.IgnoredFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }
            
            var destFile = Path.Combine(destinationDir, fileName);
            File.Copy(file, destFile, overwrite: true);
            fileCount++;
            
            if (verbose)
            {
                var relativePath = Path.GetRelativePath(sourceDir, file);
                Logger.LogVerbose($"    Copied: {relativePath}", verbose);
            }
        }

        // Recursively copy subdirectories (skip ignored directories)
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(subDir);
            if (IgnoredFilesConstants.IgnoredDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }
            
            var destSubDir = Path.Combine(destinationDir, dirName);
            fileCount += CopyDirectoryContents(subDir, destSubDir, verbose);
        }
        
        return fileCount;
    }
}
