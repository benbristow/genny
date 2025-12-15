using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class SiteGenerationPipelineTests
{
    [Fact]
    public async Task ProcessAsync_WithSingleProcessor_ExecutesProcessor()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
            },
            Verbose = false
        };

        var processor = new BuildDirectoryCreationProcessor();
        var pipeline = new SiteGenerationPipeline()
            .AddProcessor(processor);

        try
        {
            // Act
            var result = await pipeline.ProcessAsync(context);

            // Assert
            Directory.Exists(context.SiteConfig.OutputDirectory).ShouldBeTrue();
            result.ShouldBeSameAs(context);
        }
        finally
        {
            if (Directory.Exists(context.SiteConfig.OutputDirectory))
            {
                Directory.Delete(context.SiteConfig.OutputDirectory, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithMultipleProcessors_ExecutesInOrder()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);
        var testFile = Path.Combine(buildDir, "test.txt");
        await File.WriteAllTextAsync(testFile, "test");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = buildDir
            },
            Verbose = false
        };

        var pipeline = new SiteGenerationPipeline()
            .AddProcessor(new BuildDirectoryCleanupProcessor())
            .AddProcessor(new BuildDirectoryCreationProcessor());

        try
        {
            // Act
            var result = await pipeline.ProcessAsync(context);

            // Assert
            // Cleanup should remove the directory, creation should recreate it
            Directory.Exists(buildDir).ShouldBeTrue();
            File.Exists(testFile).ShouldBeFalse(); // File should be gone after cleanup
            result.ShouldBeSameAs(context);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNoProcessors_ReturnsContextUnchanged()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig(),
            Verbose = false
        };

        var pipeline = new SiteGenerationPipeline();

        // Act
        var result = await pipeline.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }

    [Fact]
    public async Task AddProcessor_ReturnsPipelineForChaining()
    {
        // Arrange
        var pipeline = new SiteGenerationPipeline();

        // Act
        var result = pipeline
            .AddProcessor(new BuildDirectoryCleanupProcessor())
            .AddProcessor(new BuildDirectoryCreationProcessor());

        // Assert
        result.ShouldBeSameAs(pipeline);
    }
}
