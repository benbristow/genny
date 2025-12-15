using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class PageBodyExtractionProcessorTests
{
    [Fact]
    public async Task ProcessAsync_SetsPageBodyFromContent()
    {
        // Arrange
        var processor = new PageBodyExtractionProcessor();
        var context = new PageProcessingContext
        {
            Content = "<p>Page Content</p>",
            PageBody = "",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageBody.ShouldBe("<p>Page Content</p>");
    }

    [Fact]
    public async Task ProcessAsync_WithExistingPageBody_OverwritesPageBody()
    {
        // Arrange
        var processor = new PageBodyExtractionProcessor();
        var context = new PageProcessingContext
        {
            Content = "New Content",
            PageBody = "Old Body",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageBody.ShouldBe("New Content");
    }

    [Fact]
    public async Task ProcessAsync_WithEmptyContent_SetsEmptyPageBody()
    {
        // Arrange
        var processor = new PageBodyExtractionProcessor();
        var context = new PageProcessingContext
        {
            Content = "",
            PageBody = "Old Body",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageBody.ShouldBe("");
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new PageBodyExtractionProcessor();
        var context = new PageProcessingContext
        {
            Content = "Test Content",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
