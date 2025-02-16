// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.OpenApi.Models;

namespace Docfx.Build.OpenApi;

public partial class OpenApiSpec
{
    private static void CreatePages(Action<string, string, ApiPage.ApiPage> output, OpenApiDocument openApiDocument,
        OpenApiJsonConfig config, string configDirectory)
    {
        var outputFolder = Path.Combine(configDirectory, config.OutputFolder);
        Directory.CreateDirectory(outputFolder);

        var toc = CreateToc(openApiDocument.Paths, config, outputFolder);
    }
}
