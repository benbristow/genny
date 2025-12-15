using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class MinifierProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithWhitespaceBetweenTags_RemovesWhitespace()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<html>   <body>   <p>Content</p>   </body>   </html>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<html><body><p>Content</p></body></html>");
    }

    [Fact]
    public async Task ProcessAsync_WithNewlinesBetweenTags_RemovesNewlines()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<html>\n<body>\n<p>Content</p>\n</body>\n</html>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<html><body><p>Content</p></body></html>");
    }

    [Fact]
    public async Task ProcessAsync_WithTabsBetweenTags_RemovesTabs()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<html>\t<body>\t<p>Content</p>\t</body>\t</html>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<html><body><p>Content</p></body></html>");
    }

    [Fact]
    public async Task ProcessAsync_WithMultipleSpacesInText_CollapsesToSingleSpace()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<p>This    has    multiple     spaces</p>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<p>This has multiple spaces</p>");
    }

    [Fact]
    public async Task ProcessAsync_WithLeadingTrailingWhitespace_RemovesWhitespace()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "   <html><body>Content</body></html>   ",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<html><body>Content</body></html>");
    }

    [Fact]
    public async Task ProcessAsync_WithComplexHTML_MinifiesCorrectly()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = @"<html>
    <head>
        <title>   Test   Page   </title>
    </head>
    <body>
        <div>
            <p>   Content   with   spaces   </p>
        </div>
    </body>
</html>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<html><head><title>Test Page</title></head><body><div><p>Content with spaces</p></div></body></html>");
    }

    [Fact]
    public async Task ProcessAsync_WithAlreadyMinifiedContent_LeavesUnchanged()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var originalContent = "<html><body><p>Content</p></body></html>";
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
    public async Task ProcessAsync_WithSingleSpacesInText_PreservesSpaces()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<p>This is a sentence with normal spacing.</p>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<p>This is a sentence with normal spacing.</p>");
    }

    [Fact]
    public async Task ProcessAsync_WithMixedWhitespace_MinifiesCorrectly()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<div>\n\t  <span>   Text   </span>  \n\t</div>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<div><span>Text</span></div>");
    }

    [Fact]
    public async Task ProcessAsync_WithAttributes_MinifiesCorrectly()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<div   class=\"container\"   id=\"main\">   <p   class=\"text\">Content</p>   </div>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<div class=\"container\" id=\"main\"><p class=\"text\">Content</p></div>");
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "<html>   <body>Content</body>   </html>",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }

    [Fact]
    public async Task ProcessAsync_WithEmptyContent_ReturnsEmpty()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("");
    }

    [Fact]
    public async Task ProcessAsync_WithOnlyWhitespace_ReturnsEmpty()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var context = new PageProcessingContext
        {
            Content = "   \n\t   ",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("");
    }
}
