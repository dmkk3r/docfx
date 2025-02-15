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
            RunOpenApi.Exec();
        });
    }
}
