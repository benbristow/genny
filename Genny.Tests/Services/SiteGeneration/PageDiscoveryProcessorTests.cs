using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class PageDiscoveryProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithPagesDirectory_DiscoversPages()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var page1Path = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(page1Path, "<html><body>Index</body></html>");
        
        var page2Path = Path.Combine(pagesDir, "about.html");
        await File.WriteAllTextAsync(page2Path, "<html><body>About</body></html>");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir
            },
            Verbose = false
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await new PageDiscoveryProcessor().ProcessAsync(context);
            }

            // Assert
            context.Pages.Count.ShouldBe(2);
            context.Pages.ShouldContain(page1Path);
            context.Pages.ShouldContain(page2Path);
            context.PagesDirectory.ShouldBe(pagesDir);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithSubdirectoryPages_IncludesSubdirectoryPages()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var subDir = Path.Combine(pagesDir, "blog");
        Directory.CreateDirectory(subDir);
        
        var subPagePath = Path.Combine(subDir, "post.html");
        await File.WriteAllTextAsync(subPagePath, "<html><body>Post</body></html>");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir
            },
            Verbose = false
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await new PageDiscoveryProcessor().ProcessAsync(context);
            }

            // Assert
            context.Pages.Count.ShouldBe(1);
            context.Pages.ShouldContain(subPagePath);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNoPagesDirectory_ReturnsEmptyList()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir
            },
            Verbose = false
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await new PageDiscoveryProcessor().ProcessAsync(context);
            }

            // Assert
            context.Pages.Count.ShouldBe(0);
            context.PagesDirectory.ShouldBe(Path.Combine(tempDir, "pages"));
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_IgnoresBuildDirectory()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var buildDir = Path.Combine(pagesDir, "build");
        Directory.CreateDirectory(buildDir);
        
        var buildPagePath = Path.Combine(buildDir, "page.html");
        await File.WriteAllTextAsync(buildPagePath, "<html><body>Build</body></html>");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir
            },
            Verbose = false
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await new PageDiscoveryProcessor().ProcessAsync(context);
            }

            // Assert
            context.Pages.ShouldNotContain(buildPagePath);
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

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir
            },
            Verbose = false
        };

        try
        {
            // Act
            SiteGenerationContext result;
            using (TestHelpers.SuppressConsoleOutput())
            {
                result = await new PageDiscoveryProcessor().ProcessAsync(context);
            }

            // Assert
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
