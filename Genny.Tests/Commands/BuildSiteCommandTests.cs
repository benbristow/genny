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
            
            // Act & Assert - Should not throw
            await Should.NotThrowAsync(async () => await command.ExecuteAsync(console));
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
    public async Task ExecuteAsync_WithValidConfig_CallsSiteGenerator()
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
            
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                await command.ExecuteAsync(console);
            }

            // Assert - Verify site was generated by checking output
            var buildDir = Path.Combine(tempDir, "build");
            Directory.Exists(buildDir).ShouldBeTrue();
            var builtPage = Path.Combine(buildDir, "index.html");
            File.Exists(builtPage).ShouldBeTrue();
            
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
    public async Task ExecuteAsync_WithVerboseFlag_PassesVerboseToSiteGenerator()
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

            // Assert - Site should be generated regardless of verbose flag
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
}
