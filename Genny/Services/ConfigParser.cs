using Tomlyn;
using Genny.Models;

namespace Genny.Services;

public static class ConfigParser
{
    private const string ConfigFileName = "genny.toml";

    /// <summary>
    /// Parses the config file from the root directory or the specified site root
    /// </summary>
    /// <returns>The parsed config if found, null otherwise</returns>
    public static async Task<SiteConfig?> ParseConfig()
    {
        var root = FindRootDirectory();
        if (root == null)
        {
            Logger.Log("Could not find root directory with genny.toml");
            return null;
        }

        var configPath = Path.Combine(root, ConfigFileName);
        return await ParseConfigFromFileAsync(configPath);
    }

    /// <summary>
    /// Finds the root directory by recursively going up the directory tree until genny.toml is found
    /// </summary>
    /// <returns>The root directory path if found, null otherwise</returns>
    private static string? FindRootDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDirectory);

        while (directory != null)
        {
            var configPath = Path.Combine(directory.FullName, ConfigFileName);
            if (File.Exists(configPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    internal static async Task<SiteConfig?> ParseConfigFromFileAsync(string configPath)
    {
        if (!File.Exists(configPath))
        {
            Logger.Log($"Config file {configPath} not found");
            return null;
        }

        try
        {
            var tomlContent = await File.ReadAllTextAsync(configPath);
            var tomlTable = Toml.ToModel(tomlContent);

            var config = new SiteConfig();

            if (tomlTable.TryGetValue("name", out var nameObj) && nameObj is string name)
            {
                config.Name = name;
            }

            if (tomlTable.TryGetValue("description", out var descriptionObj) && descriptionObj is string description)
            {
                config.Description = description;
            }

            if (tomlTable.TryGetValue("base_url", out var baseUrlObj) && baseUrlObj is string baseUrl)
            {
                config.BaseUrl = baseUrl;
            }

            if (tomlTable.TryGetValue("generate_sitemap", out var generateSitemapObj) && generateSitemapObj is bool generateSitemap)
            {
                config.GenerateSitemap = generateSitemap;
            }

            if (tomlTable.TryGetValue("minify_output", out var minifyOutputObj) && minifyOutputObj is bool minifyOutput)
            {
                config.MinifyOutput = minifyOutput;
            }

            var rootDir = Path.GetDirectoryName(configPath);
            if (rootDir == null)
            {
                Logger.Log($"Could not determine root directory from config path: {configPath}");
                return null;
            }

            config.RootDirectory = rootDir;
            config.OutputDirectory = Path.Combine(config.RootDirectory, "build");

            return config;
        }
        catch (Exception ex)
        {
            Logger.Log($"Error parsing config file: {ex.Message}");
            return null;
        }
    }
}

