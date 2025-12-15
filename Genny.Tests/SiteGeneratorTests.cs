using Genny.Models;
using Genny.Services;

namespace Genny.Tests;

public class SiteGeneratorTests
{
    [Fact]
    public async Task GenerateSiteAsync_WithPagesDirectory_CopiesPagesToBuildDirectory()
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

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            Directory.Exists(buildDir).ShouldBeTrue();
            
            var builtIndex = Path.Combine(buildDir, "index.html");
            File.Exists(builtIndex).ShouldBeTrue();
            var indexContent = await File.ReadAllTextAsync(builtIndex);
            indexContent.ShouldBe("<html><body>Index</body></html>");
            
            var builtAbout = Path.Combine(buildDir, "about.html");
            File.Exists(builtAbout).ShouldBeTrue();
            var aboutContent = await File.ReadAllTextAsync(builtAbout);
            aboutContent.ShouldBe("<html><body>About</body></html>");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithSubdirectoryPages_PreservesDirectoryStructure()
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

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtPost = Path.Combine(buildDir, "blog", "post.html");
            File.Exists(builtPost).ShouldBeTrue();
            
            var postContent = await File.ReadAllTextAsync(builtPost);
            postContent.ShouldBe("<html><body>Post</body></html>");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithPublicDirectory_CopiesPublicContents()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        
        var assetPath = Path.Combine(publicDir, "style.css");
        await File.WriteAllTextAsync(assetPath, "body { color: red; }");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtCss = Path.Combine(buildDir, "style.css");
            File.Exists(builtCss).ShouldBeTrue();
            
            var cssContent = await File.ReadAllTextAsync(builtCss);
            cssContent.ShouldBe("body { color: red; }");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithIgnoredFiles_SkipsIgnoredFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        
        var validFile = Path.Combine(publicDir, "style.css");
        await File.WriteAllTextAsync(validFile, "body { color: red; }");
        
        var ignoredFile = Path.Combine(publicDir, ".gitignore");
        await File.WriteAllTextAsync(ignoredFile, "node_modules/");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtCss = Path.Combine(buildDir, "style.css");
            File.Exists(builtCss).ShouldBeTrue();
            
            var builtGitignore = Path.Combine(buildDir, ".gitignore");
            File.Exists(builtGitignore).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithIgnoredDirectories_SkipsIgnoredDirectories()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        
        var validDir = Path.Combine(publicDir, "assets");
        Directory.CreateDirectory(validDir);
        var validFile = Path.Combine(validDir, "image.png");
        await File.WriteAllTextAsync(validFile, "fake image data");
        
        var ignoredDir = Path.Combine(publicDir, "node_modules");
        Directory.CreateDirectory(ignoredDir);
        var ignoredFile = Path.Combine(ignoredDir, "package.js");
        await File.WriteAllTextAsync(ignoredFile, "module.exports = {};");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtImage = Path.Combine(buildDir, "assets", "image.png");
            File.Exists(builtImage).ShouldBeTrue();
            
            var builtNodeModules = Path.Combine(buildDir, "node_modules");
            Directory.Exists(builtNodeModules).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_CleansBuildDirectoryBeforeGenerating()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);
        
        var oldFile = Path.Combine(buildDir, "old.html");
        await File.WriteAllTextAsync(oldFile, "old content");
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var newPage = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(newPage, "<html><body>New</body></html>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = buildDir
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            File.Exists(oldFile).ShouldBeFalse();
            
            var newBuiltFile = Path.Combine(buildDir, "index.html");
            File.Exists(newBuiltFile).ShouldBeTrue();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithNoPagesDirectory_DoesNotThrow()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act & Assert
            using (TestHelpers.SuppressConsoleOutput())
            {
                await Should.NotThrowAsync(async () => await SiteGenerator.GenerateSiteAsync(siteConfig));
            }
            
            Directory.Exists(siteConfig.OutputDirectory).ShouldBeTrue();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithDefaultLayout_AppliesLayoutToPage()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>Site</title></head><body>{{content}}</body></html>");
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<body>Page Content</body>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtPage = Path.Combine(buildDir, "index.html");
            File.Exists(builtPage).ShouldBeTrue();
            
            var pageContent = await File.ReadAllTextAsync(builtPage);
            pageContent.ShouldContain("Page Content");
            pageContent.ShouldContain("<title>Site</title>");
            pageContent.ShouldNotContain("{{content}}");
            
            // Layouts directory should not be copied
            var builtLayouts = Path.Combine(buildDir, "layouts");
            Directory.Exists(builtLayouts).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithCustomLayout_AppliesSpecifiedLayout()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var customLayoutPath = Path.Combine(layoutsDir, "custom.html");
        await File.WriteAllTextAsync(customLayoutPath, "<html><head><title>Custom</title></head><body>{{content}}</body></html>");
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<!-- layout: custom.html --><body>Custom Page</body>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtPage = Path.Combine(buildDir, "index.html");
            File.Exists(builtPage).ShouldBeTrue();
            
            var pageContent = await File.ReadAllTextAsync(builtPage);
            pageContent.ShouldContain("Custom Page");
            pageContent.ShouldContain("<title>Custom</title>");
            pageContent.ShouldNotContain("{{content}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithNoLayout_ReturnsPageAsIs()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        var pageContent = "<html><body>No Layout</body></html>";
        await File.WriteAllTextAsync(pagePath, pageContent);

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtPage = Path.Combine(buildDir, "index.html");
            File.Exists(builtPage).ShouldBeTrue();
            
            var builtContent = await File.ReadAllTextAsync(builtPage);
            builtContent.ShouldBe(pageContent);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithLayoutsDirectory_DoesNotCopyLayoutsToBuild()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{content}}</body></html>");
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<body>Test</body>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var builtLayouts = Path.Combine(buildDir, "layouts");
            Directory.Exists(builtLayouts).ShouldBeFalse();
            
            var builtLayoutFile = Path.Combine(buildDir, "layouts", "default.html");
            File.Exists(builtLayoutFile).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithGenerateSitemapFalse_DoesNotGenerateSitemap()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Index</body></html>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build"),
            GenerateSitemap = false
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var sitemapPath = Path.Combine(buildDir, "sitemap.xml");
            File.Exists(sitemapPath).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSiteAsync_WithGenerateSitemapTrue_GeneratesSitemap()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Index</body></html>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build"),
            GenerateSitemap = true
        };

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await SiteGenerator.GenerateSiteAsync(siteConfig);
            }

            // Assert
            var buildDir = siteConfig.OutputDirectory;
            var sitemapPath = Path.Combine(buildDir, "sitemap.xml");
            File.Exists(sitemapPath).ShouldBeTrue();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
