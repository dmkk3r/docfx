// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Build.OpenApi;

namespace Docfx.App;

/// <summary>
/// Helper class to generate openapi.
/// </summary>
internal static class RunOpenApi
{
    /// <summary>
    /// Generate openapi documentation with specified settings.
    /// </summary>
    public static void Exec(OpenApiJsonConfig config, string configDirectory)
    {
        OpenApiSpec.Exec(config, configDirectory).GetAwaiter().GetResult();
    }
}
