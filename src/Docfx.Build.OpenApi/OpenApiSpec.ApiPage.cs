// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Build.ApiPage;
using Docfx.Common;
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
        Parallel.ForEach(EnumerateToc(toc), n => SaveTocNode(n.id, n.operations));

        Logger.LogInfo($"Writing OpenAPI spec to {outputFolder}");
        return;

        void SaveTocNode(string id, List<KeyValuePair<OperationType, OpenApiOperation>> operations)
        {
            var body = new List<Block>();

            foreach ((OperationType operationType, OpenApiOperation operation) in operations)
            {
                var operationId = operation.OperationId;
                var operationTitle = operation.Summary ?? operation.Description ?? operationId;

                var heading = new Api1 { api1 = operationTitle, id = operationId, };
                body.Add((Api)heading);
            }

            output(outputFolder, id,
                new ApiPage.ApiPage { title = id, languageId = "openapi", body = body.ToArray() });
        }
    }
}
