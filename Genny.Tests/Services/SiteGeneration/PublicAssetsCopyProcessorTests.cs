using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class PublicAssetsCopyProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithPublicDirectory_CopiesFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);
        
        var cssFile = Path.Combine(publicDir, "style.css");
        await File.WriteAllTextAsync(cssFile, "body { color: red; }");
        
        var jsFile = Path.Combine(publicDir, "script.js");
        await File.WriteAllTextAsync(jsFile, "console.log('test');");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = buildDir
            },
            Verbose = false
        };

        try
        {
            // Act
            var result = await new PublicAssetsCopyProcessor().ProcessAsync(context);

            // Assert
            var copiedCss = Path.Combine(buildDir, "style.css");
            var copiedJs = Path.Combine(buildDir, "script.js");
            
            File.Exists(copiedCss).ShouldBeTrue();
            File.Exists(copiedJs).ShouldBeTrue();
            
            (await File.ReadAllTextAsync(copiedCss)).ShouldBe("body { color: red; }");
            (await File.ReadAllTextAsync(copiedJs)).ShouldBe("console.log('test');");
            
            result.CopiedPublicFiles.ShouldBe(2);
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNoPublicDirectory_DoesNothing()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = buildDir
            },
            Verbose = false
        };

        try
        {
            // Act
            var result = await new PublicAssetsCopyProcessor().ProcessAsync(context);

            // Assert
            result.CopiedPublicFiles.ShouldBe(0);
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithSubdirectories_CopiesRecursively()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        
        var imagesDir = Path.Combine(publicDir, "images");
        Directory.CreateDirectory(imagesDir);
        
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);
        
        var imageFile = Path.Combine(imagesDir, "logo.png");
        await File.WriteAllTextAsync(imageFile, "fake image data");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = buildDir
            },
            Verbose = false
        };

        try
        {
            // Act
            var result = await new PublicAssetsCopyProcessor().ProcessAsync(context);

            // Assert
            var copiedImage = Path.Combine(buildDir, "images", "logo.png");
            File.Exists(copiedImage).ShouldBeTrue();
            result.CopiedPublicFiles.ShouldBe(1);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_IgnoresIgnoredFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);
        
        var cssFile = Path.Combine(publicDir, "style.css");
        await File.WriteAllTextAsync(cssFile, "body { color: red; }");
        
        var gitignoreFile = Path.Combine(publicDir, ".gitignore");
        await File.WriteAllTextAsync(gitignoreFile, "ignored");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = buildDir
            },
            Verbose = false
        };

        try
        {
            // Act
            var result = await new PublicAssetsCopyProcessor().ProcessAsync(context);

            // Assert
            var copiedCss = Path.Combine(buildDir, "style.css");
            var copiedGitignore = Path.Combine(buildDir, ".gitignore");
            
            File.Exists(copiedCss).ShouldBeTrue();
            File.Exists(copiedGitignore).ShouldBeFalse(); // Should be ignored
            result.CopiedPublicFiles.ShouldBe(1);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = buildDir
            },
            Verbose = false
        };

        try
        {
            // Act
            var result = await new PublicAssetsCopyProcessor().ProcessAsync(context);

            // Assert
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
