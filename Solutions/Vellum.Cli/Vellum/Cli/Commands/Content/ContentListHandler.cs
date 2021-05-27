// <copyright file="ContentListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    using Vellum.Abstractions;
    using Vellum.Abstractions.Content;
    using Vellum.Abstractions.Content.ContentFactories;
    using Vellum.Abstractions.Content.Primitives;
    using Vellum.Abstractions.Taxonomy;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;

    public static class ContentListHandler
    {
        public static async Task<int> ExecuteAsync(
            IServiceCollection services,
            ListOptions options,
            IVellumConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            services.AddWellKnownTaxonomyContentTypes();
            services.AddWellKnownContentFragmentTypeFactories();
            services.AddWellKnownContentBlockContentTypes();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var siteTaxonomyRepository = new SiteDetailsRepository();
            SiteDetails siteTaxonomy = await siteTaxonomyRepository.FindAsync(options.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

            var taxonomyDocumentRepository = new TaxonomyDocumentRespository(services);
            var siteTaxonomyParser = new SiteTaxonomyParser();

            IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments = taxonomyDocumentRepository.LoadAllAsync(options.SiteTaxonomyDirectoryPath);
            IAsyncEnumerable<TaxonomyDocument> loaded = taxonomyDocumentRepository.LoadContentFragmentsAsync(taxonomyDocuments);

            // List<TaxonomyDocument> documents = await loaded.Select(x => x).ToListAsync();
            await foreach (TaxonomyDocument doc in loaded)
            {
                // Console.WriteLine(doc.Path.ToString());
                foreach (ContentFragment contentFragment in doc.ContentFragments)
                {
                    if (contentFragment.ContentType == WellKnown.ContentFragments.ContentTypes.BlogMarkdown)
                    {
                        ContentFragmentTypeFactory<IBlogPost> cff = serviceProvider.GetContent<ContentFragmentTypeFactory<IBlogPost>>(contentFragment.ContentType.AsContentFragmentFactory());
                        IBlogPost cf = cff.Create(contentFragment);
                        Console.WriteLine(cf.Title);
                    }
                }
            }

            /*NavigationNode siteNavigation = siteTaxonomyParser.Parse(documents);

            var siteContext = new SiteContext
            {
                Preview = false,
                Navigation = siteNavigation,
                Pages = documents,
                Details = siteTaxonomy,
            };*/

            stopwatch.Stop();

            /*
            var siteTaxonomyRepository = new SiteDetailsRepository();

            SiteDetails siteTaxonomy = await siteTaxonomyRepository.FindAsync(options.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

            console.Out.Write(siteTaxonomy.Title + System.Environment.NewLine);
            */
            Console.WriteLine("Rendering Took: {0}", stopwatch.Elapsed);

            return ReturnCodes.Ok;
        }
    }
}

/*await foreach (TaxonomyDocument doc in loaded)
{
    Console.WriteLine(doc.Path.ToString());
    foreach (ContentFragment contentFragment in doc.ContentFragments)
    {
        if (contentFragment.ContentType == WellKnown.ContentFragments.ContentTypes.BlogMarkdown)
        {
            ContentFragmentTypeFactory<IBlogPost> cff = serviceProvider.GetContent<ContentFragmentTypeFactory<IBlogPost>>(contentFragment.ContentType.AsContentFragmentFactory());
            IBlogPost cf = cff.Create(contentFragment);
            Console.WriteLine(cf.Attachments);
            Console.WriteLine(cf.Author);
            Console.WriteLine(cf.Body);
            Console.WriteLine(cf.Categories);
            Console.WriteLine(cf.ContentType);
            Console.WriteLine(cf.Date);
            Console.WriteLine(cf.Excerpt);
            Console.WriteLine(cf.Faqs);
            Console.WriteLine(cf.HeaderImageUrl);
            Console.WriteLine(cf.IsSeries);
            Console.WriteLine(cf.PartTitle);
            Console.WriteLine(cf.Position);
            Console.WriteLine(cf.Profile);
            Console.WriteLine(cf.PublicationStatus);
            Console.WriteLine(cf.Series);
            Console.WriteLine(cf.Slug);
            Console.WriteLine(cf.Tags);
            Console.WriteLine(cf.Title);
            Console.WriteLine(cf.Url);
            Console.WriteLine(cf.UserName);
        }
    }
}*/