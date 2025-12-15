using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class TitleExtractionProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithTitleComment_ExtractsTitleFromComment()
    {
        // Arrange
        var processor = new TitleExtractionProcessor();
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
        var processor = new TitleExtractionProcessor();
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
        var processor = new TitleExtractionProcessor();
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
        var processor = new TitleExtractionProcessor();
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
    public async Task ProcessAsync_WithSpacesInTitleComment_TrimsWhitespace()
    {
        // Arrange
        var processor = new TitleExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- title:   My Title   --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageTitle.ShouldBe("My Title");
    }

    [Fact]
    public async Task ProcessAsync_WithCaseInsensitiveTitleComment_ExtractsTitle()
    {
        // Arrange
        var processor = new TitleExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- TITLE: My Title --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageTitle.ShouldBe("My Title");
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new TitleExtractionProcessor();
        var context = new PageProcessingContext
        {
            OriginalPageContent = "<!-- title: Test -->",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
