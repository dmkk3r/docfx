// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Spectre.Console.Cli;

namespace Docfx;

[Description("Generate OpenAPI documentation from a specification file")]
internal class OpenApiCommandOptions : LogOptions
{
    [Description("Specify the file to generate OpenAPI documentation from")]
    [CommandOption("-f|--file")]
    public string File { get; set; }
}
