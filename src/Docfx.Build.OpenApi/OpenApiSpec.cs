// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Docfx.Common;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using YamlDotNet.Serialization;

namespace Docfx.Build.OpenApi;

public static partial class OpenApiSpec
{
    public static Task Exec(OpenApiJsonConfig config, string configDirectory)
    {
        var openApiDocument = ReadOpenApiSpec(config, configDirectory);

        if (openApiDocument == null)
        {
            return Task.CompletedTask;
        }

        var serializer = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
        CreatePages(WriteYaml, openApiDocument, config, configDirectory);

        return Task.CompletedTask;

        void WriteYaml(string outputFolder, string id, ApiPage.ApiPage apiPage)
        {
            var json = JsonSerializer.Serialize(apiPage, ApiPage.ApiPage.JsonSerializerOptions);
            var obj = serializer.Deserialize(json);
            YamlUtility.Serialize(Path.Combine(outputFolder, $"{id}.yml"), obj, "YamlMime:ApiPage");
        }
    }

    private static OpenApiDocument? ReadOpenApiSpec(OpenApiJsonConfig config, string configDirectory)
    {
        var specFile = config.SpecFile;
        var stream = File.OpenRead(specFile);

        var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

        if (diagnostic.SpecificationVersion != OpenApiSpecVersion.OpenApi3_0)
        {
            Logger.LogError(
                $"The OpenAPI spec version is not supported. Supported version is {OpenApiSpecVersion.OpenApi3_0}");
            return null;
        }

        foreach (var warning in diagnostic.Warnings)
        {
            Logger.LogWarning($"OpenAPI spec warning: {warning.Message}");
        }

        foreach (var error in diagnostic.Errors)
        {
            Logger.LogError($"OpenAPI spec error: {error.Message}");
            return null;
        }

        return openApiDocument;
    }
}
