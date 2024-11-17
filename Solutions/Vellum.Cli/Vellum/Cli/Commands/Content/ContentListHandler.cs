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
            Stopwatch stopwatch = new();
            stopwatch.Start();

            services.AddWellKnownTaxonomyContentTypes();
            services.AddWellKnownContentFragmentTypeFactories();
            services.AddWellKnownContentBlockContentTypes();
            services.AddWellKnownConverterFactories();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            TaxonomyDocumentListExtension.Configure(serviceProvider);

            SiteDetailsRepository siteTaxonomyRepository = new();
            SiteDetails siteTaxonomy = await siteTaxonomyRepository.FindAsync(siteTaxonomyDirectoryPath).ConfigureAwait(false);

            TaxonomyDocumentRespository taxonomyDocumentRepository = new(services);
            SiteTaxonomyParser siteTaxonomyParser = new();

            IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments = taxonomyDocumentRepository.LoadAllAsync(siteTaxonomyDirectoryPath);
            List<TaxonomyDocument> loaded = await taxonomyDocumentRepository.LoadContentFragmentsAsync(taxonomyDocuments).ToListAsync();

            List<IAuthor> authors = loaded.GetAllAuthors();
            List<IBlogPost> blogs = loaded.GetAllBlogPosts();

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

            NavigationNode siteNavigation = siteTaxonomyParser.Parse(loaded);

            SiteContext siteContext = new()
            {
                Preview = false,
                Navigation = siteNavigation,
                Pages = loaded,
                Details = siteTaxonomy,
            };

            stopwatch.Stop();

            Console.WriteLine("Rendering Took: {0}", stopwatch.Elapsed);
            Console.ReadKey();

            return ReturnCodes.Ok;
        }
    }
}