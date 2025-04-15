// <copyright file="ContentListCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Primitives;
using Vellum.Abstractions.Taxonomy;
using Vellum.Cli.Abstractions;
using Vellum.Rendering.Scriban;

namespace Vellum.Cli.Commands.Content;

using System.Collections.Generic;
using System.Linq;

public class ContentListCommand(IServiceCollection services) : AsyncCommand<ContentListCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
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
        SiteDetails? siteTaxonomy = await siteTaxonomyRepository.FindAsync(settings.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

        TaxonomyDocumentRespository taxonomyDocumentRepository = new(services);
        SiteTaxonomyParser siteTaxonomyParser = new();

        IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments = taxonomyDocumentRepository.LoadAllAsync(settings.SiteTaxonomyDirectoryPath);
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
            string status = post.PublicationStatus.ToString();

            // Filter by draft/published status if specified
            if ((settings.Draft && post.PublicationStatus == PublicationStatus.Draft) ||
                (settings.Published && post.PublicationStatus == PublicationStatus.Published) ||
                settings is { Draft: false, Published: false })
            {
                table.AddRow(
                    post.Title,
                    post.Author(authors).Email,
                    post.Date.ToShortDateString(),
                    status);
            }
        }

        AnsiConsole.Write(table);

        NavigationNode siteNavigation = siteTaxonomyParser.Parse(loaded);

        /*SiteContext siteContext = new()
        {
            Preview = false,
            Navigation = siteNavigation,
            Pages = loaded!,
            Details = siteTaxonomy!,
        };*/

        stopwatch.Stop();

        ScribanRenderer renderer = new();

        await renderer.RenderAsync(@"c:\temp\scriban\test.html", "<html><head><title>{{title}} - {{date}}</title></head></html>", blogs.First());

        AnsiConsole.WriteLine($"Rendering Took: {stopwatch.Elapsed}");

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        [CommandOption("--draft|-d")]
        [Description("Show only draft content")]
        public bool Draft { get; set; }

        [CommandOption("--published|-p")]
        [Description("Show only published content")]
        public bool Published { get; set; }

        [CommandOption("--site-path|-s")]
        [Description("Path to the site taxonomy directory")]
        public DirectoryPath SiteTaxonomyDirectoryPath { get; set; } = null!;
    }
}