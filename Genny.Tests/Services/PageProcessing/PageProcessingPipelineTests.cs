using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class PageProcessingPipelineTests
{
    private static PageProcessingContext CreateContext(string content)
    {
        return new PageProcessingContext
        {
            Content = content,
            PageBody = "Body",
            PageTitle = "Title",
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
    public async Task ProcessAsync_WithSingleProcessor_ExecutesProcessor()
    {
        // Arrange
        var pipeline = new PageProcessingPipeline()
            .AddProcessor(new PlaceholderReplacementProcessor());
        var context = CreateContext("{{ title }}");

        // Act
        var result = await pipeline.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Title");
    }

    [Fact]
    public async Task ProcessAsync_WithMultipleProcessors_ExecutesInOrder()
    {
        // Arrange
        var pipeline = new PageProcessingPipeline()
            .AddProcessor(new PlaceholderReplacementProcessor())
            .AddProcessor(new PlaceholderReplacementProcessor()); // Run twice
        var context = CreateContext("{{ title }}");

        // Act
        var result = await pipeline.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Title");
    }

    [Fact]
    public async Task ProcessAsync_WithPlaceholderAndPartialProcessors_ProcessesBoth()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        var partialPath = Path.Combine(partialsDir, "header.html");
        await File.WriteAllTextAsync(partialPath, "<h1>{{ site.name }}</h1>");
        
        var pipeline = new PageProcessingPipeline()
            .AddProcessor(new PlaceholderReplacementProcessor())
            .AddProcessor(new PartialInclusionProcessor());
        var context = CreateContext("{{ partial: header.html }}");
        context.RootDirectory = tempDir;

        try
        {
            // Act
            var result = await pipeline.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe("<h1>Test Site</h1>");
            result.Content.ShouldNotContain("{{");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithEmptyPipeline_ReturnsOriginalContent()
    {
        // Arrange
        var pipeline = new PageProcessingPipeline();
        var context = CreateContext("Original Content");

        // Act
        var result = await pipeline.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Original Content");
    }

    [Fact]
    public async Task ProcessAsync_WithChainedProcessors_PassesResultThroughChain()
    {
        // Arrange
        var pipeline = new PageProcessingPipeline()
            .AddProcessor(new PlaceholderReplacementProcessor())
            .AddProcessor(new PlaceholderReplacementProcessor());
        var context = CreateContext("{{ title }} - {{ site.name }}");

        // Act
        var result = await pipeline.ProcessAsync(context);

        // Assert
        result.Content.ShouldBe("Title - Test Site");
    }

    [Fact]
    public async Task AddProcessor_ReturnsPipelineForChaining()
    {
        // Arrange
        var pipeline = new PageProcessingPipeline();

        // Act
        var result = pipeline.AddProcessor(new PlaceholderReplacementProcessor());

        // Assert
        result.ShouldBeSameAs(pipeline);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var pipeline = new PageProcessingPipeline()
            .AddProcessor(new PlaceholderReplacementProcessor());
        var context = CreateContext("{{ title }}");

        // Act
        var result = await pipeline.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
