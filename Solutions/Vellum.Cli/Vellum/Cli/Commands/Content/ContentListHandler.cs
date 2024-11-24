// <copyright file="ContentListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine.Invocation;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NDepend.Path;
    using Spectre.Console;
    using Vellum.Abstractions.Content;
    using Vellum.Abstractions.Content.ContentFactories;
    using Vellum.Abstractions.Content.Primitives;
    using Vellum.Abstractions.Taxonomy;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Infrastructure;

    public static class ContentListHandler
    {
        public static async Task<int> ExecuteAsync(
            bool draft,
            bool published,
            IAbsoluteDirectoryPath siteTaxonomyDirectoryPath,
            IServiceCollection services,
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            services.AddWellKnownTaxonomyContentTypes();
            services.AddWellKnownContentFragmentTypeFactories();
            services.AddWellKnownContentBlockContentTypes();
            services.AddWellKnownConverterFactories();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var siteTaxonomyRepository = new SiteDetailsRepository();
            SiteDetails siteTaxonomy = await siteTaxonomyRepository.FindAsync(siteTaxonomyDirectoryPath).ConfigureAwait(false);

            var taxonomyDocumentRepository = new TaxonomyDocumentRespository(services);
            var siteTaxonomyParser = new SiteTaxonomyParser();

            IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments = taxonomyDocumentRepository.LoadAllAsync(siteTaxonomyDirectoryPath);
            List<TaxonomyDocument> loaded = await taxonomyDocumentRepository.LoadContentFragmentsAsync(taxonomyDocuments).ToListAsync();

            List<IAuthor> authors = loaded.GetAllAuthors(serviceProvider);
            List<IBlogPost> blogs = loaded.GetAllBlogPosts(serviceProvider);

            Table table = new();
            table.AddColumn("Title");
            table.AddColumn("Author");
            table.AddColumn("Date");
            table.AddColumn("Status");

            foreach (IBlogPost post in blogs)
            {
                table.AddRow(
                    post.Title,
                    post.Author(authors).Email,
                    post.Date.ToShortDateString(),
                    post.PublicationStatus.ToString());
            }

            console.Write(table);

            List<TaxonomyDocument> docs = loaded;

            NavigationNode siteNavigation = siteTaxonomyParser.Parse(docs);

            var siteContext = new SiteContext
            {
                Preview = false,
                Navigation = siteNavigation,
                Pages = docs,
                Details = siteTaxonomy,
            };

            stopwatch.Stop();

            Console.WriteLine("Rendering Took: {0}", stopwatch.Elapsed);
            Console.ReadKey();
            return ReturnCodes.Ok;
        }
    }
}