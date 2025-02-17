// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Build.OpenApi.Toc;
using Docfx.Common;
using Microsoft.OpenApi.Models;

namespace Docfx.Build.OpenApi;

public partial class OpenApiSpec
{
    private static List<OpenApiTocNode> CreateToc(OpenApiDocument document, OpenApiJsonConfig config,
        string outputFolder)
    {
        List<OpenApiTocNode> toc = [];

        CreateInformationToc(document.Info);

        var tags = document.Paths
            .SelectMany(p => p.Value.Operations.SelectMany(o => o.Value.Tags))
            .Distinct()
            .ToList();

        foreach (var tag in tags)
        {
            var tagNode = new OpenApiTocNode
            {
                id = tag.Name, name = tag.Name, type = OpenApiTocNodeType.Tag, href = $"{tag.Name}.yml"
            };

            toc.Add(tagNode);

            foreach (var path in document.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    if (!operation.Value.Tags.Contains(tag))
                    {
                        continue;
                    }

                    var id = operation.Value.OperationId;

                    var operationNode = new OpenApiTocNode
                    {
                        id = id,
                        name = operation.Value.Summary ?? operation.Value.Description ?? id,
                        type = OpenApiTocNodeType.Operation,
                        href = $"{id}.yml"
                    };

                    operationNode.operations.Add(operation);

                    tagNode.items ??= [];
                    tagNode.items.Add(operationNode);
                    tagNode.operations.Add(operation);
                }
            }

            tagNode.containsLeafNodes = tagNode.items?.Count != 0;
        }

        var tocPath = Path.Combine(outputFolder, "toc.yml");
        YamlUtility.Serialize(tocPath, toc, YamlMime.TableOfContent);

        return toc;

        void CreateInformationToc(OpenApiInfo info)
        {
            var informationNode = new OpenApiTocNode
            {
                id = "information",
                name = "Information",
                type = OpenApiTocNodeType.Info,
                href = "information.yml"
            };

            toc.Add(informationNode);
        }
    }

    private static IEnumerable<(string id, List<KeyValuePair<OperationType, OpenApiOperation>> operations)>
        EnumerateToc(List<OpenApiTocNode> items)
    {
        foreach (var item in items)
        {
            if (item.items is not null)
                foreach (var i in EnumerateToc(item.items))
                    yield return i;

            if (item.id is not null && item.operations.Count > 0)
                yield return (item.id, item.operations);
        }
    }
}
