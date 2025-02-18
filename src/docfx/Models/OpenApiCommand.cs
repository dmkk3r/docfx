// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.App;
using Spectre.Console.Cli;

namespace Docfx;

internal class OpenApiCommand : Command<OpenApiCommandOptions>
{
    public override int Execute(CommandContext context, OpenApiCommandOptions options)
    {
        return CommandHelper.Run(options, () =>
        {
            (DocfxConfig config, string baseDirectory) = Docset.GetConfig(options.Config);
            MergeOptionsToConfig(options, config.openapi, baseDirectory);
            RunOpenApi.Exec(config.openapi, baseDirectory);
        });
    }

    private static void MergeOptionsToConfig(OpenApiCommandOptions options, OpenApiJsonConfig config, string configDirectory)
    {
        config.SpecFile = options.SpecFile ?? config.SpecFile;
        config.OutputFolder = options.OutputFolder ?? config.OutputFolder;
    }
}
