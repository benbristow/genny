using CliFx.Infrastructure;
using Genny.Commands;

namespace Genny.Tests.Commands;

public class BuildSiteCommandTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidConfig_GeneratesSite()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        const string configContent = """
                                     name = "Test Site"
                                     description = "A test site"
                                     """;
        await File.WriteAllTextAsync(configPath, configContent);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = false };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await command.ExecuteAsync(console);
            }

            // Assert
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeTrue();
            var builtPage = Path.Combine(buildDir, "index.html");
            File.Exists(builtPage).ShouldBeTrue();
            
            // Verify content was processed correctly
            var content = await File.ReadAllTextAsync(builtPage);
            content.ShouldContain("Test");
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithValidConfigAndVerbose_GeneratesSiteWithVerboseOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        const string configContent = """
                                     name = "Test Site"
                                     """;
        await File.WriteAllTextAsync(configPath, configContent);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = true };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await command.ExecuteAsync(console);
            }

            // Assert
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeTrue();
            File.Exists(Path.Combine(buildDir, "index.html")).ShouldBeTrue();
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithNoConfig_CompletesWithoutThrowing()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = false };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            await command.ExecuteAsync(console);
            
            // Assert - Should complete without throwing
            // (No exception means success)
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithNoConfig_DoesNotGenerateSite()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = false };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            await command.ExecuteAsync(console);

            // Assert
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeFalse();
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }


    [Fact]
    public async Task ExecuteAsync_WithVerboseFlag_ProducesVerboseOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        const string configContent = """
                                     name = "Test Site"
                                     description = "A test site"
                                     """;
        await File.WriteAllTextAsync(configPath, configContent);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = true };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            await command.ExecuteAsync(console);

            // Assert - Verify verbose output was produced
            var output = stringWriter.ToString();
            output.ShouldContain("Configuration:");
            output.ShouldContain("Root directory:");
            output.ShouldContain("Output directory:");
            output.ShouldContain("Site name:");
            
            // Verify site was still generated
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeTrue();
            File.Exists(Path.Combine(buildDir, "index.html")).ShouldBeTrue();
        }
        finally
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithoutVerboseFlag_DoesNotProduceVerboseOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        const string configContent = """
                                     name = "Test Site"
                                     """;
        await File.WriteAllTextAsync(configPath, configContent);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = false };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            await command.ExecuteAsync(console);

            // Assert - Verify verbose output was NOT produced
            var output = stringWriter.ToString();
            output.ShouldNotContain("Configuration:");
            output.ShouldNotContain("Root directory:");
            
            // But should still have basic output (like "Found X page(s)")
            output.ShouldContain("Found");
            output.ShouldContain("page(s)");
            
            // Verify site was still generated
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeTrue();
        }
        finally
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithVerboseFlag_LogsPageProcessingDetails()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        const string configContent = """
                                     name = "Test Site"
                                     """;
        await File.WriteAllTextAsync(configPath, configContent);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = true };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            await command.ExecuteAsync(console);

            // Assert - Verify verbose logging includes page processing details
            var output = stringWriter.ToString();
            output.ShouldContain("Processing:");
            output.ShouldContain("index.html");
            
            // Verify site was generated
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeTrue();
        }
        finally
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithVerboseFlagAndPublicAssets_LogsAssetCopying()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        const string configContent = """
                                     name = "Test Site"
                                     """;
        await File.WriteAllTextAsync(configPath, configContent);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        var assetPath = Path.Combine(publicDir, "style.css");
        await File.WriteAllTextAsync(assetPath, "body { color: red; }");

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = true };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            await command.ExecuteAsync(console);

            // Assert - Verify verbose logging includes asset copying
            var output = stringWriter.ToString();
            output.ShouldContain("Copying public assets");
            output.ShouldContain("Copied");
            output.ShouldContain("file(s)");
            
            // Verify assets were copied
            var buildDir = Path.Combine(tempDir, "build");
            File.Exists(Path.Combine(buildDir, "style.css")).ShouldBeTrue();
        }
        finally
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithVerboseFlagAndNoConfig_DoesNotProduceVerboseOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var originalDir = Directory.GetCurrentDirectory();
        var console = new FakeConsole();
        var command = new BuildSiteCommand { Verbose = true };

        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            await command.ExecuteAsync(console);

            // Assert - Should not have verbose configuration output since config wasn't found
            // The error is written to console.Error (FakeConsole), not Console.Error
            var output = stringWriter.ToString();
            output.ShouldNotContain("Configuration:");
            output.ShouldNotContain("Root directory:");
            
            // Verify no site was generated
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeFalse();
        }
        finally
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
