using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class PermalinkCalculationProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithFilePath_CalculatesPermalink()
    {
        // Arrange
        var processor = new PermalinkCalculationProcessor();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "about.html");
        
        var context = new PageProcessingContext
        {
            FilePath = pagePath,
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Permalink.ShouldBe("/about.html");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithIndexHtml_CalculatesRootPermalink()
    {
        // Arrange
        var processor = new PermalinkCalculationProcessor();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        
        var context = new PageProcessingContext
        {
            FilePath = pagePath,
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Permalink.ShouldBe("/");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithBaseUrl_CalculatesFullUrl()
    {
        // Arrange
        var processor = new PermalinkCalculationProcessor();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "about.html");
        
        var context = new PageProcessingContext
        {
            FilePath = pagePath,
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig
            {
                BaseUrl = "https://example.com"
            },
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Permalink.ShouldBe("https://example.com/about.html");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithSubdirectoryPage_CalculatesSubdirectoryPermalink()
    {
        // Arrange
        var processor = new PermalinkCalculationProcessor();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var blogDir = Path.Combine(pagesDir, "blog");
        Directory.CreateDirectory(blogDir);
        var pagePath = Path.Combine(blogDir, "post.html");
        
        var context = new PageProcessingContext
        {
            FilePath = pagePath,
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Permalink.ShouldBe("/blog/post.html");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithEmptyFilePath_DoesNotCalculatePermalink()
    {
        // Arrange
        var processor = new PermalinkCalculationProcessor();
        var context = new PageProcessingContext
        {
            FilePath = string.Empty,
            RootDirectory = "/test",
            SiteConfig = new SiteConfig(),
            Permalink = "original",
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Permalink.ShouldBe("original");
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new PermalinkCalculationProcessor();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "test.html");
        
        var context = new PageProcessingContext
        {
            FilePath = pagePath,
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
