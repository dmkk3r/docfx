// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Build.ApiPage;
using Docfx.Build.OpenApi.Vendor;
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

        SaveInformationNode();

        var toc = CreateToc(openApiDocument, config, outputFolder);
        Parallel.ForEach(EnumerateToc(toc), n => SaveTocNode(n.id, n.operations));

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
                new ApiPage.ApiPage { title = id, languageId = "json", body = body.ToArray() });
        }
    }
}
