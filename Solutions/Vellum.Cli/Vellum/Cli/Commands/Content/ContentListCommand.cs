// <copyright file="ContentListCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using NDepend.Path;

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Primitives;
using Vellum.Abstractions.Taxonomy;
using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Commands.Content;

using System.Collections.Generic;
using System.Linq;

public class ContentListCommand : AsyncCommand<ContentListCommand.Settings>
{
    private readonly IAppEnvironment appEnvironment;
    private readonly IServiceCollection services;

    public ContentListCommand(IAppEnvironment appEnvironment, IServiceCollection services)
    {
        this.appEnvironment = appEnvironment;
        this.services = services;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        this.services.AddWellKnownTaxonomyContentTypes();
        this.services.AddWellKnownContentFragmentTypeFactories();
        this.services.AddWellKnownContentBlockContentTypes();
        this.services.AddWellKnownConverterFactories();

        ServiceProvider serviceProvider = this.services.BuildServiceProvider();

        TaxonomyDocumentListExtension.Configure(serviceProvider);

        SiteDetailsRepository siteTaxonomyRepository = new();
        SiteDetails siteTaxonomy = await siteTaxonomyRepository.FindAsync(settings.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

        TaxonomyDocumentRespository taxonomyDocumentRepository = new(this.services);
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
                (!settings.Draft && !settings.Published))
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

        SiteContext siteContext = new()
        {
            Preview = false,
            Navigation = siteNavigation,
            Pages = loaded,
            Details = siteTaxonomy,
        };

        stopwatch.Stop();

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
        public IAbsoluteDirectoryPath SiteTaxonomyDirectoryPath { get; set; } = null!;
    }
}