using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class BuildDirectoryCreationProcessorTests
{
    [Fact]
    public async Task ProcessAsync_CreatesBuildDirectory()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var buildDir = Path.Combine(tempDir, "build");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = buildDir
            }
        };

        try
        {
            // Act
            var result = await new BuildDirectoryCreationProcessor().ProcessAsync(context);

            // Assert
            Directory.Exists(buildDir).ShouldBeTrue();
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
    public async Task ProcessAsync_WithExistingDirectory_DoesNotThrow()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = buildDir
            }
        };

        try
        {
            // Act
            var result = await new BuildDirectoryCreationProcessor().ProcessAsync(context);

            // Assert
            Directory.Exists(buildDir).ShouldBeTrue();
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
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var buildDir = Path.Combine(tempDir, "build");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = buildDir
            }
        };

        try
        {
            // Act
            var result = await new BuildDirectoryCreationProcessor().ProcessAsync(context);

            // Assert
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
}
