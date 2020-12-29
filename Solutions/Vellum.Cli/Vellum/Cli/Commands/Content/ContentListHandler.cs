// <copyright file="ContentListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using Vellum.Abstractions;
    using Vellum.Abstractions.Content.ContentFactories;
    using Vellum.Abstractions.IO;
    using Vellum.Abstractions.Taxonomy;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;

    public static class ContentListHandler
    {
        public static async Task<int> ExecuteAsync(
            IServiceCollection services,
            ListOptions options,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            services.AddWellKnownTaxonomyContentTypes();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var taxonomyFileInfoRepository = new TaxonomyFileInfoRepository();
            var siteTaxonomyRepository = new SiteTaxonomyRepository();

            IAsyncEnumerable<TaxonomyFileInfo> files = taxonomyFileInfoRepository.FindAllAsync(options.SiteTaxonomyDirectoryPath);

            var taxonomyDocuments = new List<TaxonomyDocument>();

            await foreach (TaxonomyFileInfo file in files)
            {
                IFileReader<TaxonomyDocument> reader = serviceProvider.GetContent<IFileReader<TaxonomyDocument>>(file.ContentType);

                if (reader != null)
                {
                    TaxonomyDocument taxonomyDocument = await reader.ReadAsync(file.Path).ConfigureAwait(false);
                    taxonomyDocument.Hash = file.Hash;
                    taxonomyDocuments.Add(taxonomyDocument);
                }
                else
                {
                    console.Error.Write($"Cannot Read file with ContentType {file.ContentType}" + System.Environment.NewLine);
                }
            }

            SiteTaxonomy siteTaxonomy = await siteTaxonomyRepository.FindAsync(options.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

            console.Out.Write(siteTaxonomy.Title + System.Environment.NewLine);

            return ReturnCodes.Ok;
        }
    }
}
