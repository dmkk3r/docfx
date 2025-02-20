// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Build.OpenApi.Toc;
using Docfx.Common;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
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
            var tagName = tag.Extensions.TryGetValue("x-displayName", out var displayName)
                ? (displayName as OpenApiString)?.Value
                : tag.Name;

            var tagNode = new OpenApiTocNode
            {
                id = tag.Name, name = tagName, type = OpenApiTocNodeType.Tag, href = $"{tag.Name}.yml"
            };

            tagNode.openApiElements.Add(tag);

            toc.Add(tagNode);

            foreach (var path in document.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    if (!operation.Value.Tags.Contains(tag))
                    {
                        continue;
                    }

                    var id = GetCleanedId(operation.Value.OperationId ?? operation.Value.Summary);

                    var operationNode = new OpenApiTocNode
                    {
                        id = id,
                        name = operation.Value.Summary ?? operation.Value.Description ?? id,
                        type = OpenApiTocNodeType.Operation,
                        href = $"{id}.yml"
                    };

                    operationNode.openApiElements.Add(operation.Value);

                    tagNode.items ??= [];
                    tagNode.items.Add(operationNode);
                    tagNode.openApiElements.Add(operation.Value);
                }
            }
        }

        var tocPath = Path.Combine(outputFolder, "toc.yml");
        YamlUtility.Serialize(tocPath, toc, YamlMime.TableOfContent);

        return toc;

        void CreateInformationToc(OpenApiInfo info)
        {
            var informationNode = new OpenApiTocNode
            {
                id = "information", name = "Information", type = OpenApiTocNodeType.Info, href = "information.yml"
            };

            toc.Add(informationNode);
        }

        string GetCleanedId(string? id)
        {
            if (id is null)
                return string.Empty;

            if (id.Any(char.IsWhiteSpace))
                return id.Replace(" ", "-").ToLower();

            return string.Concat(id.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x : x.ToString()))
                .ToLower();
        }
    }

    private static IEnumerable<(string id, OpenApiTocNodeType type, List<IOpenApiElement> openApiElements)>
        EnumerateToc(List<OpenApiTocNode> items)
    {
        foreach (var item in items)
        {
            if (item.items is not null)
                foreach (var i in EnumerateToc(item.items))
                    yield return i;

            if (item.id is not null && item.openApiElements.Count > 0)
                yield return (item.id, item.type, item.openApiElements);
        }
    }
}
