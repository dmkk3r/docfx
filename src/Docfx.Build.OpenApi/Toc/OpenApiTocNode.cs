// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.OpenApi.Models;

namespace Docfx.Build.OpenApi.Toc;

internal class OpenApiTocNode
{
    public string name { get; init; } = "";
    public string? href { get; init; }
    public List<OpenApiTocNode>? items { get; set; }

    internal OpenApiTocNodeType type;
    internal string? id;
    internal bool containsLeafNodes;
    internal List<KeyValuePair<OperationType, OpenApiOperation>> operations = [];
}
