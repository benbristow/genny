using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class PlaceholderReplacementProcessorTests
{
    private static PageProcessingContext CreateContext(string content, string pageBody = "", string pageTitle = "", bool verbose = false)
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
            IncludedPartials = [],
            Verbose = verbose
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

    [Fact]
    public async Task ProcessAsync_WithVerboseFlagAndReplacements_LogsReplacementCount()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ title }} {{ site.name }}", pageTitle: "Title", verbose: true);

        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            await processor.ProcessAsync(context);

            // Assert
            var output = stringWriter.ToString();
            output.ShouldContain("Replaced");
            output.ShouldContain("placeholder(s)");
        }
    }

    [Fact]
    public async Task ProcessAsync_WithVerboseFlagAndNoReplacements_DoesNotLog()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("No placeholders here", verbose: true);

        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            await processor.ProcessAsync(context);

            // Assert
            var output = stringWriter.ToString();
            output.ShouldNotContain("Replaced");
            output.ShouldNotContain("placeholder(s)");
        }
    }

    [Fact]
    public async Task ProcessAsync_WithoutVerboseFlag_DoesNotLog()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ title }} {{ site.name }}", pageTitle: "Title", verbose: false);

        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            await processor.ProcessAsync(context);

            // Assert
            var output = stringWriter.ToString();
            output.ShouldNotContain("Replaced");
            output.ShouldNotContain("placeholder(s)");
        }
    }

    [Fact]
    public async Task ProcessAsync_WithVerboseFlag_LogsCorrectReplacementCount()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext(
            "{{ title }}{{ title }}{{ site.name }}{{ year }}",
            pageTitle: "Title",
            verbose: true
        );

        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            await processor.ProcessAsync(context);

            // Assert - Should log 4 replacements (2 title, 1 site.name, 1 year)
            var output = stringWriter.ToString();
            output.ShouldContain("Replaced 4 placeholder(s)");
        }
    }

    [Fact]
    public async Task ProcessAsync_WithVerboseFlag_StillReplacesPlaceholders()
    {
        // Arrange
        var processor = new PlaceholderReplacementProcessor();
        var context = CreateContext("{{ title }}", pageTitle: "My Title", verbose: true);

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert - Placeholders should still be replaced regardless of verbose flag
        result.Content.ShouldBe("My Title");
    }
}
