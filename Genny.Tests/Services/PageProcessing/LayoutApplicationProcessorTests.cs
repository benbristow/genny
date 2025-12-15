using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class LayoutApplicationProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithExistingLayout_AppliesLayout()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "custom.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{ content }}</body></html>");
        
        var processor = new LayoutApplicationProcessor();
        var context = new PageProcessingContext
        {
            Content = "<p>Page Body</p>",
            PageBody = "<p>Page Body</p>",
            LayoutName = "custom.html",
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe("<html><body>{{ content }}</body></html>");
            result.PageBody.ShouldBe("<p>Page Body</p>");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithDefaultLayout_AppliesDefaultLayout()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{ content }}</body></html>");
        
        var processor = new LayoutApplicationProcessor();
        var context = new PageProcessingContext
        {
            Content = "<p>Page Body</p>",
            PageBody = "<p>Page Body</p>",
            LayoutName = "default.html",
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe("<html><body>{{ content }}</body></html>");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithMissingLayout_LeavesContentUnchanged()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var processor = new LayoutApplicationProcessor();
        var originalContent = "<p>Page Body</p>";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            PageBody = originalContent,
            LayoutName = "missing.html",
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe(originalContent);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNoLayoutsDirectory_LeavesContentUnchanged()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var processor = new LayoutApplicationProcessor();
        var originalContent = "<p>Page Body</p>";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            PageBody = originalContent,
            LayoutName = "default.html",
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe(originalContent);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_PreservesPageBody()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{ content }}</body></html>");
        
        var processor = new LayoutApplicationProcessor();
        var pageBody = "<p>Preserved Body</p>";
        var context = new PageProcessingContext
        {
            Content = pageBody,
            PageBody = pageBody,
            LayoutName = "default.html",
            RootDirectory = tempDir,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.PageBody.ShouldBe(pageBody);
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
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{ content }}</body></html>");
        
        var processor = new LayoutApplicationProcessor();
        var context = new PageProcessingContext
        {
            Content = "<p>Body</p>",
            PageBody = "<p>Body</p>",
            LayoutName = "default.html",
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
