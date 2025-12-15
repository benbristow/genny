using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class SitemapGenerationProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithSitemapEnabled_GeneratesSitemap()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);
        
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Index</body></html>");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = buildDir,
                GenerateSitemap = true
            },
            Pages = [pagePath],
            PagesDirectory = pagesDir,
            Verbose = false
        };

        try
        {
            // Act
            var result = await new SitemapGenerationProcessor().ProcessAsync(context);

            // Assert
            var sitemapPath = Path.Combine(buildDir, "sitemap.xml");
            File.Exists(sitemapPath).ShouldBeTrue();
            
            var sitemapContent = await File.ReadAllTextAsync(sitemapPath);
            sitemapContent.ShouldContain("sitemap");
            // index.html gets converted to "/" in sitemap
            sitemapContent.ShouldContain("<loc>");
            
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithSitemapDisabled_DoesNotGenerateSitemap()
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
                OutputDirectory = buildDir,
                GenerateSitemap = false
            },
            Verbose = false
        };

        try
        {
            // Act
            var result = await new SitemapGenerationProcessor().ProcessAsync(context);

            // Assert
            var sitemapPath = Path.Combine(buildDir, "sitemap.xml");
            File.Exists(sitemapPath).ShouldBeFalse();
            result.ShouldBeSameAs(context);
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
                OutputDirectory = buildDir,
                GenerateSitemap = false
            },
            Verbose = false
        };

        try
        {
            // Act
            var result = await new SitemapGenerationProcessor().ProcessAsync(context);

            // Assert
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
