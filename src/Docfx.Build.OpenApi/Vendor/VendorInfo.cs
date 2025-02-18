// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Docfx.Build.OpenApi.Vendor;

public class VendorInfo
{
    private const string _xLogo = "x-logo";

    public (string? url, string? alt) XLogo { get; init; }

    public VendorInfo(OpenApiInfo info)
    {
        if (info.Extensions.TryGetValue(_xLogo, out var value) && value is OpenApiObject xLogo)
        {
            var url = xLogo.TryGetValue("url", out var urlValue)
                      && urlValue is OpenApiString urlString
                ? urlString.Value
                : null;

            var alt = xLogo.TryGetValue("alt", out var altValue)
                      && altValue is OpenApiString altString
                ? altString.Value
                : null;

            XLogo = (url, alt);
        }
    }
}
