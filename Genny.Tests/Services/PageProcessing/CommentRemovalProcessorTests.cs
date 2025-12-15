using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class CommentRemovalProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithLayoutComment_RemovesLayoutComment()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var context = new PageProcessingContext
        {
            Content = "<!-- layout: custom.html --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<body>Content</body>");
        result.Content.ShouldNotContain("<!-- layout:");
    }

    [Fact]
    public async Task ProcessAsync_WithTitleComment_RemovesTitleComment()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var context = new PageProcessingContext
        {
            Content = "<!-- title: My Title --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<body>Content</body>");
        result.Content.ShouldNotContain("<!-- title:");
    }

    [Fact]
    public async Task ProcessAsync_WithBothComments_RemovesBoth()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var context = new PageProcessingContext
        {
            Content = "<!-- layout: custom.html --><!-- title: My Title --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<body>Content</body>");
        result.Content.ShouldNotContain("<!-- layout:");
        result.Content.ShouldNotContain("<!-- title:");
    }

    [Fact]
    public async Task ProcessAsync_WithWhitespaceAroundComments_RemovesWhitespace()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var context = new PageProcessingContext
        {
            Content = "   <!-- layout: custom.html -->   <body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<body>Content</body>");
    }

    [Fact]
    public async Task ProcessAsync_WithNoComments_LeavesContentUnchanged()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var originalContent = "<body>Content</body>";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe(originalContent);
    }

    [Fact]
    public async Task ProcessAsync_WithCaseInsensitiveComments_RemovesComments()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var context = new PageProcessingContext
        {
            Content = "<!-- LAYOUT: custom.html --><!-- TITLE: My Title --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<body>Content</body>");
    }

    [Fact]
    public async Task ProcessAsync_SetsPageBodyAfterRemovingComments()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var context = new PageProcessingContext
        {
            Content = "<!-- layout: custom.html --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.PageBody.ShouldBe("<body>Content</body>");
        result.PageBody.ShouldBe(result.Content);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new CommentRemovalProcessor();
        var context = new PageProcessingContext
        {
            Content = "<!-- layout: test.html --><body>Content</body>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
