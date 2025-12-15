using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class PlaceholderReplacementProcessorTests
{
    private static PageProcessingContext CreateContext(string content, string pageBody = "", string pageTitle = "")
    {
        return new PageProcessingContext
        {
            Content = content,
            PageBody = pageBody,
            PageTitle = pageTitle,
            SiteConfig = new SiteConfig
            {
                Name = "Test Site",
                Description = "Test Description"
            },
            CurrentYear = "2025",
            CurrentEpoch = "1734201600",
            Permalink = "/test.html",
            RootDirectory = "/test",
            IncludedPartials = []
        };
    }

    [Fact]
    public async Task ProcessAsync_WithContentPlaceholder_ReplacesWithPageBody()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ content }}", "Page Body Content");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Page Body Content");
    }

    [Fact]
    public async Task ProcessAsync_WithTitlePlaceholder_ReplacesWithPageTitle()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ title }}", pageTitle: "My Page Title");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("My Page Title");
    }

    [Fact]
    public async Task ProcessAsync_WithSiteNamePlaceholder_ReplacesWithSiteName()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ site.name }}");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Test Site");
    }

    [Fact]
    public async Task ProcessAsync_WithSiteDescriptionPlaceholder_ReplacesWithSiteDescription()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ site.description }}");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Test Description");
    }

    [Fact]
    public async Task ProcessAsync_WithYearPlaceholder_ReplacesWithCurrentYear()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ year }}");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("2025");
    }

    [Fact]
    public async Task ProcessAsync_WithEpochPlaceholder_ReplacesWithCurrentEpoch()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ epoch }}");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("1734201600");
    }

    [Fact]
    public async Task ProcessAsync_WithPermalinkPlaceholder_ReplacesWithPermalink()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ permalink }}");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("/test.html");
    }

    [Fact]
    public async Task ProcessAsync_WithMultiplePlaceholders_ReplacesAll()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext(
            "<h1>{{ title }}</h1><p>{{ site.name }}</p><span>{{ year }}</span>",
            pageTitle: "My Title"
        );

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("<h1>My Title</h1><p>Test Site</p><span>2025</span>");
    }

    [Fact]
    public async Task ProcessAsync_WithSpacesInPlaceholders_ReplacesCorrectly()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{  title  }}", pageTitle: "Title");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Title");
    }

    [Fact]
    public async Task ProcessAsync_WithCaseInsensitivePlaceholders_ReplacesCorrectly()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ TITLE }}", pageTitle: "Title");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Title");
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ title }}", pageTitle: "Title");

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
