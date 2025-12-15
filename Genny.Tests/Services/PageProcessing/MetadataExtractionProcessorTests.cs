using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class MetadataExtractionProcessorTests
{
    [Fact]
    public async Task ProcessAsync_SetsCurrentYear()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "test",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.CurrentYear.ShouldBe(DateTime.Now.Year.ToString());
    }

    [Fact]
    public async Task ProcessAsync_SetsCurrentEpoch()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var beforeEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "test",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);
        var afterEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Assert
        var epochValue = long.Parse(result.CurrentEpoch);
        epochValue.ShouldBeGreaterThanOrEqualTo(beforeEpoch);
        epochValue.ShouldBeLessThanOrEqualTo(afterEpoch + 1); // Allow 1 second tolerance
    }

    [Fact]
    public async Task ProcessAsync_WithTitleComment_ExtractsTitleFromComment()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- title: My Page Title --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageTitle.ShouldBe("My Page Title");
    }

    [Fact]
    public async Task ProcessAsync_WithTitleTag_ExtractsTitleFromTag()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<html><head><title>Page Title</title></head><body>Content</body></html>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageTitle.ShouldBe("Page Title");
    }

    [Fact]
    public async Task ProcessAsync_WithTitleCommentAndTag_PrefersComment()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- title: Comment Title --><html><head><title>Tag Title</title></head></html>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageTitle.ShouldBe("Comment Title");
    }

    [Fact]
    public async Task ProcessAsync_WithNoTitle_KeepsEmptyString()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<body>Content</body>",
            PageTitle = "",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageTitle.ShouldBe("");
    }

    [Fact]
    public async Task ProcessAsync_WithLayoutComment_ExtractsLayoutName()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
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
        var processor = new MetadataExtractionProcessor();
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
    public async Task ProcessAsync_ExtractsAllMetadata()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- title: My Title --><!-- layout: custom.html --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.CurrentYear.ShouldBe(DateTime.Now.Year.ToString());
        result.CurrentEpoch.ShouldNotBeNullOrEmpty();
        result.PageTitle.ShouldBe("My Title");
        result.LayoutName.ShouldBe("custom.html");
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "test",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
