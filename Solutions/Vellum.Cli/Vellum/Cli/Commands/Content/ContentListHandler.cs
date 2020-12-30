// <copyright file="ContentListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Vellum.Abstractions.Content;
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
            var taxonomyDocumentRepository = new TaxonomyDocumentRespository(services);

            IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments = taxonomyDocumentRepository.LoadAllAsync(options.SiteTaxonomyDirectoryPath);
            IAsyncEnumerable<TaxonomyDocument> loaded = taxonomyDocumentRepository.LoadContentFragmentsAsync(taxonomyDocuments);

            await foreach (TaxonomyDocument doc in loaded)
            {
                Console.WriteLine(doc.Path.ToString());
                foreach (ContentFragment contentFragment in doc.ContentFragments)
                {
                    Console.WriteLine(contentFragment.Id);
                }
            }

            /*
            var siteTaxonomyRepository = new SiteTaxonomyRepository();

            SiteTaxonomy siteTaxonomy = await siteTaxonomyRepository.FindAsync(options.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

            console.Out.Write(siteTaxonomy.Title + System.Environment.NewLine);
            */

            return ReturnCodes.Ok;
        }
    }
}
