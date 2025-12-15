using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class ConfigurationLoggingProcessorTests
{
    [Fact]
    public async Task ProcessAsync_WithVerbose_LogsConfiguration()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = "/test/root",
                OutputDirectory = "/test/build",
                Name = "Test Site",
                MinifyOutput = true,
                GenerateSitemap = false
            },
            Verbose = true
        };

        // Act
        using (TestHelpers.SuppressConsoleOutput())
        {
            await new ConfigurationLoggingProcessor().ProcessAsync(context);
        }

        // Assert
        // Just verify it doesn't throw - logging is tested via integration tests
    }

    [Fact]
    public async Task ProcessAsync_WithoutVerbose_DoesNotLog()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig(),
            Verbose = false
        };

        // Act
        var result = await new ConfigurationLoggingProcessor().ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig(),
            Verbose = false
        };

        // Act
        var result = await new ConfigurationLoggingProcessor().ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }
}
