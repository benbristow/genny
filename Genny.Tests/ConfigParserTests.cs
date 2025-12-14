using Genny.Services;

namespace Genny.Tests;

public class ConfigParserTests
{
    [Fact]
    public async Task ParseConfig_WithValidToml_ReturnsSiteConfig()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        var configContent = """
            name = "Test Site"
            description = "A test site"
            """;
        await File.WriteAllTextAsync(configPath, configContent);

        var originalDir = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            var config = await ConfigParser.ParseConfig();

            // Assert
            config.ShouldNotBeNull();
            config.Name.ShouldBe("Test Site");
            config.Description.ShouldBe("A test site");
            config.RootDirectory.ShouldBe(tempDir);
            config.OutputDirectory.ShouldBe(Path.Combine(tempDir, "build"));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ParseConfig_WithInvalidToml_ReturnsNull()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        const string invalidContent = "this is not valid toml [";
        await File.WriteAllTextAsync(configPath, invalidContent);

        var originalDir = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            var config = await ConfigParser.ParseConfig();

            // Assert
            config.ShouldBeNull();
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ParseConfig_WithPartialConfig_ReturnsConfigWithDefaults()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var configPath = Path.Combine(tempDir, "genny.toml");
        var configContent = "name = \"Partial Site\"";
        await File.WriteAllTextAsync(configPath, configContent);

        var originalDir = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            var config = await ConfigParser.ParseConfig();

            // Assert
            config.ShouldNotBeNull();
            config.Name.ShouldBe("Partial Site");
            config.Description.ShouldBe(string.Empty);
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ParseConfig_WithNoConfigFile_ReturnsNull()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var originalDir = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(tempDir);
            
            // Act
            var config = await ConfigParser.ParseConfig();

            // Assert
            config.ShouldBeNull();
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
