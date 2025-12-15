using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class LayoutExtractionProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithLayoutComment_ExtractsLayoutName()
    {
        // Arrange
        var processor = new LayoutExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- layout: custom.html --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.LayoutName.ShouldBe("custom.html");
    }

    [Fact]
    public async Task ProcessAsync_WithNoLayoutComment_UsesDefaultLayout()
    {
        // Arrange
        var processor = new LayoutExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.LayoutName.ShouldBe("default.html");
    }

    [Fact]
    public async Task ProcessAsync_WithSpacesInLayoutComment_TrimsWhitespace()
    {
        // Arrange
        var processor = new LayoutExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- layout:   custom.html   --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.LayoutName.ShouldBe("custom.html");
    }

    [Fact]
    public async Task ProcessAsync_WithCaseInsensitiveLayoutComment_ExtractsLayout()
    {
        // Arrange
        var processor = new LayoutExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- LAYOUT: custom.html --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.LayoutName.ShouldBe("custom.html");
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new LayoutExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- layout: test.html -->",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
