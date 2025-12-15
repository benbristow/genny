using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class CompletionLoggingProcessorTests
{
    [Fact]
    public async Task ProcessAsync_LogsCompletionMessage()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = "/test/build"
            },
            Verbose = false
        };

        // Act
        using (TestHelpers.SuppressConsoleOutput())
        {
            await new CompletionLoggingProcessor().ProcessAsync(context);
        }

        // Assert
        // Just verify it doesn't throw - logging is tested via integration tests
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
        SiteGenerationContext result;
        using (TestHelpers.SuppressConsoleOutput())
        {
            result = await new CompletionLoggingProcessor().ProcessAsync(context);
        }

        // Assert
        result.ShouldBeSameAs(context);
    }
}
