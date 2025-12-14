using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Genny.Services;

namespace Genny.Commands;

[Command("build", Description = "Build the site")]
public class BuildSiteCommand : ICommand
{
    [CommandOption("verbose", 'v', Description = "Enable verbose output")]
    public bool Verbose { get; init; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var siteConfig = await ConfigParser.ParseConfig();
        if (siteConfig == null)
        {
            await console.Error.WriteLineAsync("Could not find site config");
            return;
        }

        if (Verbose)
        {
            await console.Output.WriteLineAsync($"Site name: {siteConfig.Name}");
            await console.Output.WriteLineAsync($"Site description: {siteConfig.Description}");
        }

        await SiteGenerator.GenerateSiteAsync(siteConfig, Verbose);
        await console.Output.WriteLineAsync("Building site");

    }
}