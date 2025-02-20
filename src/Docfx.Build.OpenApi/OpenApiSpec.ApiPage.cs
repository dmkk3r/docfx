// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Build.ApiPage;
using Docfx.Build.OpenApi.Toc;
using Docfx.Build.OpenApi.Vendor;
using Docfx.Common;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Docfx.Build.OpenApi;

public partial class OpenApiSpec
{
    private static void CreatePages(Action<string, string, ApiPage.ApiPage> output, OpenApiDocument openApiDocument,
        OpenApiJsonConfig config, string configDirectory)
    {
        var outputFolder = Path.Combine(configDirectory, config.OutputFolder);
        Directory.CreateDirectory(outputFolder);

        SaveInformationNode();

        var toc = CreateToc(openApiDocument, config, outputFolder);
        Parallel.ForEach(EnumerateToc(toc), n => SaveTocNode(n.id, n.type, n.openApiElements));

        Logger.LogInfo($"Writing OpenAPI spec to {outputFolder}");
        return;

        void SaveInformationNode()
        {
            var vendorInfo = new VendorInfo(openApiDocument.Info);

            List<Block> blocks = [];

            Title();
            Logo();
            Description();
            TermsOfService();
            Contact();
            License();

            output(outputFolder, "information",
                new ApiPage.ApiPage { title = "Information", languageId = "openapi", body = blocks.ToArray() });

            return;

            void Title()
            {
                var title = $"{openApiDocument.Info.Title} ({openApiDocument.Info.Version})";
                Api api = new Api1 { api1 = title, id = "information" };
                blocks.Add(api);
            }

            void Logo()
            {
                if (vendorInfo.XLogo.url is not null)
                {
                    blocks.Add(new Markdown { markdown = $"![{vendorInfo.XLogo.alt}]({vendorInfo.XLogo.url})" });
                }
            }

            void Description()
            {
                if (openApiDocument.Info.Description is not null)
                {
                    blocks.Add(new Markdown { markdown = openApiDocument.Info.Description });
                }
            }

            void TermsOfService()
            {
                if (openApiDocument.Info.TermsOfService is null)
                {
                    return;
                }

                Api termsOfService = new Api2 { api2 = "Terms of Service", id = "terms-of-service" };
                Markdown markdown = new Markdown { markdown = openApiDocument.Info.TermsOfService.ToString() };
                blocks.Add(termsOfService);
                blocks.Add(markdown);
            }

            void Contact()
            {
                if (openApiDocument.Info.Contact is null)
                {
                    return;
                }

                var contact = openApiDocument.Info.Contact;

                Api contactApi = new Api2 { api2 = "Contact", id = "contact" };
                Markdown contactMarkdown = new() { markdown = $"{contact.Name}: [{contact.Email}]({contact.Email})" };
                Markdown urlMarkdown = new() { markdown = $"URL: [{contact.Url}]({contact.Url})" };

                blocks.Add(contactApi);
                blocks.Add(contactMarkdown);
                blocks.Add(urlMarkdown);
            }

            void License()
            {
                if (openApiDocument.Info.License is null)
                {
                    return;
                }

                var license = openApiDocument.Info.License;

                Api licenseApi = new Api2 { api2 = "License", id = "license" };
                Markdown licenseMarkdown = new() { markdown = $"[{license.Name}]({license.Url})" };

                blocks.Add(licenseApi);
                blocks.Add(licenseMarkdown);
            }
        }

        void SaveTocNode(string id, OpenApiTocNodeType type, List<IOpenApiElement> openApiElements)
        {
            var body = new List<Block>();

            switch (type)
            {
                case OpenApiTocNodeType.Tag:
                    var tag = openApiElements.FirstOrDefault(e => e is OpenApiTag) as OpenApiTag;

                    if (tag is null)
                        return;

                    var tagName = tag.Extensions.TryGetValue("x-displayName", out var displayName)
                        ? (displayName as OpenApiString)?.Value
                        : tag.Name;

                    body.Add((Api)new Api1 { api1 = tagName!, id = tag.Name });
                    body.Add(new Markdown { markdown = tag.Description });

                    foreach (var operation in openApiElements.OfType<OpenApiOperation>())
                    {
                        body.Add(new Markdown
                        {
                            markdown =
                                $"[{operation.Summary ?? operation.Description ?? operation.OperationId}]({GetCleanedId(operation.OperationId ?? operation.Summary)}.html)"
                        });
                    }

                    break;
                case OpenApiTocNodeType.Operation:
                    if (openApiElements.First() is not OpenApiOperation firstOperation)
                        return;

                    body.Add((Api)new Api1
                    {
                        api1 = firstOperation.Summary ?? firstOperation.Description ?? firstOperation.OperationId,
                        id = firstOperation.OperationId
                    });
                    break;
                case OpenApiTocNodeType.Info:
                case OpenApiTocNodeType.Path:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            output(outputFolder, id,
                new ApiPage.ApiPage { title = id, languageId = "json", body = body.ToArray() });

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
    }
}
