using CliFx;
using Genny.Commands;

return await new CliApplicationBuilder()
    .AddCommand<BuildSiteCommand>()
    .SetTitle("Genny")
    .SetExecutableName("Genny")
    .SetDescription("A static site generator")
    .Build()
    .RunAsync(args);
