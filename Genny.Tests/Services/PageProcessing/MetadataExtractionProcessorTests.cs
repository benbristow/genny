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
            Content = "test",
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
            Content = "test",
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
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var processor = new MetadataExtractionProcessor();
        var context = new PageProcessingContext
        {
            Content = "test",
            SiteConfig = new SiteConfig(),
            IncludedPartials = []
        };

        // Act
        var result = await processor.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
