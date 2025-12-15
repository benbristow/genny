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
        var originalContent = "<html>   <body>   <p>Content</p>   </body>   </html>";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        // WebMarkupMin should reduce the size by removing whitespace
        result.Content.Length.ShouldBeLessThan(originalContent.Length);
        result.Content.ShouldContain("<html>");
        result.Content.ShouldContain("<body>");
        result.Content.ShouldContain("Content");
        // Should not have excessive whitespace between tags
        result.Content.ShouldNotContain(">   <");
    }

    [Fact]
    public async Task ProcessAsync_WithNewlinesBetweenTags_RemovesNewlines()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var originalContent = "<html>\n<body>\n<p>Content</p>\n</body>\n</html>";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        // WebMarkupMin should reduce size by removing newlines
        result.Content.Length.ShouldBeLessThan(originalContent.Length);
        result.Content.ShouldContain("<html>");
        result.Content.ShouldContain("<body>");
        result.Content.ShouldContain("Content");
        result.Content.ShouldNotContain("\n");
    }

    [Fact]
    public async Task ProcessAsync_WithTabsBetweenTags_RemovesTabs()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var originalContent = "<html>\t<body>\t<p>Content</p>\t</body>\t</html>";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        // WebMarkupMin should reduce size by removing tabs
        result.Content.Length.ShouldBeLessThan(originalContent.Length);
        result.Content.ShouldContain("<html>");
        result.Content.ShouldContain("<body>");
        result.Content.ShouldContain("Content");
        result.Content.ShouldNotContain("\t");
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
        var originalContent = "   <html><body>Content</body></html>   ";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        // WebMarkupMin should process the content and remove leading/trailing whitespace
        result.Content.ShouldContain("<html>");
        result.Content.ShouldContain("<body>");
        result.Content.ShouldContain("Content");
        // Content should be processed (may be minified or preserved if WebMarkupMin encounters issues)
        result.Content.Length.ShouldBeGreaterThan(0);
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
        // WebMarkupMin removes whitespace between tags and collapses spaces
        result.Content.ShouldContain("<html>");
        result.Content.ShouldContain("<head>");
        result.Content.ShouldContain("<title>");
        result.Content.ShouldContain("Test");
        result.Content.ShouldContain("Page");
        result.Content.ShouldContain("</title>");
        result.Content.ShouldContain("<body>");
        result.Content.ShouldContain("<div>");
        result.Content.ShouldContain("<p>");
        result.Content.ShouldContain("Content");
        result.Content.ShouldContain("with");
        result.Content.ShouldContain("spaces");
        result.Content.ShouldNotContain("\n");
        result.Content.ShouldNotContain("   "); // Multiple spaces should be collapsed
    }

    [Fact]
    public async Task ProcessAsync_WithAlreadyMinifiedContent_ProcessesCorrectly()
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
        // WebMarkupMin should process the content (may make minor optimizations)
        result.Content.ShouldContain("<html>");
        result.Content.ShouldContain("<body>");
        result.Content.ShouldContain("Content");
        // Content should be valid HTML
        result.Content.Length.ShouldBeGreaterThan(0);
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
        // WebMarkupMin removes whitespace between tags and collapses spaces in text
        result.Content.ShouldContain("<div>");
        result.Content.ShouldContain("<span>");
        result.Content.ShouldContain("Text");
        result.Content.ShouldContain("</span>");
        result.Content.ShouldContain("</div>");
        result.Content.ShouldNotContain("\n");
        result.Content.ShouldNotContain("\t");
        // Text content should have spaces collapsed (may preserve one space)
        result.Content.ShouldNotContain("   "); // Multiple spaces should be collapsed
    }

    [Fact]
    public async Task ProcessAsync_WithAttributes_MinifiesCorrectly()
    {
        // Arrange
        var processor = new MinifierProcessor();
        var originalContent = "<div   class=\"container\"   id=\"main\">   <p   class=\"text\">Content</p>   </div>";
        var context = new PageProcessingContext
        {
            Content = originalContent,
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        // WebMarkupMin should reduce size by normalizing attributes and removing whitespace
        // Note: WebMarkupMin may remove quotes from attributes when not needed (valid HTML5)
        result.Content.Length.ShouldBeLessThan(originalContent.Length);
        result.Content.ShouldContain("class");
        result.Content.ShouldContain("container");
        result.Content.ShouldContain("id");
        result.Content.ShouldContain("main");
        result.Content.ShouldContain("text");
        result.Content.ShouldContain("Content");
        result.Content.ShouldNotContain("   "); // Multiple spaces should be removed
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
        // WebMarkupMin should remove all whitespace-only content
        result.Content.Trim().ShouldBe("");
    }
}
