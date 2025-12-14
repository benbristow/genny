using CliFx;
using Genny.Commands;

return await new CliApplicationBuilder()
    .AddCommand<BuildSiteCommand>()
    .SetTitle("Genny")
    .SetDescription("A static site generator")
    .Build()
    .RunAsync(args);
