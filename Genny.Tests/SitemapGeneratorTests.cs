using Genny.Models;
using Genny.Services;
using System.Xml.Linq;

namespace Genny.Tests;

public class SitemapGeneratorTests
{
    [Fact]
    public async Task GenerateSitemapAsync_WithPages_GeneratesValidSitemap()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var page1Path = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(page1Path, "<body>Index</body>");
        
        var page2Path = Path.Combine(pagesDir, "about.html");
        await File.WriteAllTextAsync(page2Path, "<body>About</body>");

        var pages = new List<string> { page1Path, page2Path };

        try
        {
            // Act
            var sitemapContent = await SitemapGenerator.GenerateSitemapAsync(pages, pagesDir);

            // Assert
            sitemapContent.ShouldNotBeNull();
            sitemapContent.ShouldContain("urlset");
            // index.html maps to root URL
            sitemapContent.ShouldContain("<loc>/</loc>");
            sitemapContent.ShouldContain("<loc>/about.html</loc>");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSitemapAsync_WithBaseUrl_UsesBaseUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<body>Index</body>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build"),
            BaseUrl = "https://example.com"
        };

        var pages = new List<string> { pagePath };

        try
        {
            // Act
            var sitemapContent = await SitemapGenerator.GenerateSitemapAsync(pages, pagesDir, siteConfig.BaseUrl);

            // Assert
            sitemapContent.ShouldNotBeNull();
            sitemapContent.ShouldContain("https://example.com");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSitemapAsync_WithSubdirectoryPages_IncludesAllPages()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var subDir = Path.Combine(pagesDir, "blog");
        Directory.CreateDirectory(subDir);
        
        var rootPage = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(rootPage, "<body>Index</body>");
        
        var subPage = Path.Combine(subDir, "post.html");
        await File.WriteAllTextAsync(subPage, "<body>Post</body>");

        var pages = new List<string> { rootPage, subPage };

        try
        {
            // Act
            var sitemapContent = await SitemapGenerator.GenerateSitemapAsync(pages, pagesDir);

            // Assert
            sitemapContent.ShouldNotBeNull();
            // index.html maps to root URL
            sitemapContent.ShouldContain("<loc>/</loc>");
            sitemapContent.ShouldContain("blog/post.html");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSitemapAsync_WithIndexHtml_GeneratesRootUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<body>Index</body>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build"),
            BaseUrl = "https://example.com"
        };

        var pages = new List<string> { pagePath };

        try
        {
            Directory.CreateDirectory(siteConfig.OutputDirectory);
            var sitemapPath = Path.Combine(siteConfig.OutputDirectory, "sitemap.xml");
            
            // Act
            var sitemapContent = await SitemapGenerator.GenerateSitemapAsync(pages, pagesDir, siteConfig.BaseUrl);
            if (sitemapContent != null)
            {
                await File.WriteAllTextAsync(sitemapPath, sitemapContent);
            }

            // Assert
            var fileContent = await File.ReadAllTextAsync(sitemapPath);
            // index.html should map to root URL
            fileContent.ShouldContain("<loc>https://example.com</loc>");
            fileContent.ShouldNotContain("index.html");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSitemapAsync_WithNoPages_DoesNotGenerateSitemap()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        var pages = new List<string>();

        try
        {
            Directory.CreateDirectory(siteConfig.OutputDirectory);
            
            // Act
            var sitemapContent = await SitemapGenerator.GenerateSitemapAsync(pages, Path.Combine(tempDir, "pages"));

            // Assert
            sitemapContent.ShouldBeNull();
            var sitemapPath = Path.Combine(siteConfig.OutputDirectory, "sitemap.xml");
            File.Exists(sitemapPath).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task GenerateSitemapAsync_GeneratesValidXml()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<body>Index</body>");

        var siteConfig = new SiteConfig
        {
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        var pages = new List<string> { pagePath };

        try
        {
            Directory.CreateDirectory(siteConfig.OutputDirectory);
            var sitemapPath = Path.Combine(siteConfig.OutputDirectory, "sitemap.xml");
            
            // Act
            var sitemapContent = await SitemapGenerator.GenerateSitemapAsync(pages, pagesDir);
            if (sitemapContent != null)
            {
                await File.WriteAllTextAsync(sitemapPath, sitemapContent);
            }

            // Assert
            File.Exists(sitemapPath).ShouldBeTrue();
            
            // Verify it's valid XML
            var doc = XDocument.Load(sitemapPath);
            doc.Root.ShouldNotBeNull();
            doc.Root.Name.LocalName.ShouldBe("urlset");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}

