// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Build.OpenApi.Toc;
using Docfx.Common;
using Microsoft.OpenApi.Models;

namespace Docfx.Build.OpenApi;

public partial class OpenApiSpec
{
    private static List<OpenApiTocNode> CreateToc(OpenApiPaths paths, OpenApiJsonConfig config, string outputFolder)
    {
        List<OpenApiTocNode> toc = [];

        var tags = paths
            .SelectMany(p => p.Value.Operations.SelectMany(o => o.Value.Tags))
            .Distinct()
            .ToList();

        foreach (var tag in tags)
        {
            var tagNode = new OpenApiTocNode { name = tag.Name, type = OpenApiTocNodeType.Tag };

            toc.Add(tagNode);

            foreach (var path in paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    if (!operation.Value.Tags.Contains(tag))
                    {
                        continue;
                    }

                    var operationNode = new OpenApiTocNode
                    {
                        name = operation.Value.Summary, type = OpenApiTocNodeType.Operation
                    };

                    tagNode.items ??= [];
                    tagNode.items.Add(operationNode);
                }
            }

            tagNode.containsLeafNodes = tagNode.items?.Count != 0;
        }

        var tocPath = Path.Combine(outputFolder, "toc.yml");
        YamlUtility.Serialize(tocPath, toc, YamlMime.TableOfContent);

        return toc;
    }
}
