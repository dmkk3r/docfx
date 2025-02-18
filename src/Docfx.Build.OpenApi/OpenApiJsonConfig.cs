// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Docfx;

/// <summary>
/// OpenApiJsonConfig.
/// </summary>
public class OpenApiJsonConfig
{
    /// <summary>
    /// The OpenAPI specification file.
    /// </summary>
    [JsonProperty("specFile")]
    [JsonPropertyName("specFile")]
    public string SpecFile { get; set; }

    /// <summary>
    /// The output folder for the generated OpenAPI documentation.
    /// </summary>
    [JsonProperty("output")]
    [JsonPropertyName("output")]
    public string OutputFolder { get; set; }
}
