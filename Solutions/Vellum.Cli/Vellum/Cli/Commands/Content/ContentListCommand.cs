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

using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Cli.Abstractions;
using Vellum.Middleware;
using Vellum.Middleware.Abstractions;

namespace Vellum.Cli.Commands.Content;

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

        IVellumBuilder builder = new VellumBuilder();
        builder.UseStandardMiddleware(services, settings.SiteTaxonomyDirectoryPath, settings.OutputDirectoryPath);

        IVellumPipeline pipeline = builder.Build();

        VellumContext initialContext = new();

        await pipeline.ExecuteAsync(initialContext);

#pragma warning disable SA1123 // Do not place regions within elements
        #region old
        /*
        SiteDetailsRepository siteDetailsRepository = new();
        SiteDetails? siteDetails = await siteDetailsRepository.FindAsync(settings.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

        TaxonomyDocumentRepository taxonomyDocumentRepository = new(services);

        IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments = taxonomyDocumentRepository.LoadAllAsync(settings.SiteTaxonomyDirectoryPath);
        List<TaxonomyDocument> loaded = await taxonomyDocumentRepository.LoadContentFragmentsAsync(taxonomyDocuments).ToListAsync();

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        TaxonomyDocumentListExtension.Configure(serviceProvider);
        List<ContentFragment> foo = loaded.GetAllBlogPostsWithAuthorsAsContentFragments();
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
                    post.AuthorId,
                    post.Date.ToShortDateString(),
                    status);
            }
        }

        AnsiConsole.Write(table);

        SiteTaxonomyParser siteTaxonomyParser = new();
        NavigationNode siteNavigation = siteTaxonomyParser.Parse(loaded);

        SiteContext siteContext = new()
        {
            Preview = false,
            Navigation = siteNavigation,
            Pages = loaded!,
            Details = siteTaxonomy!,
        };
        ScribanRenderer renderer = new();
        await renderer.RenderAsync(@"c:\temp\scriban\test.html", "<html><head><title>{{title}} - {{date}}</title></head></html>", blogs.First());
        */
        #endregion

        stopwatch.Stop();
#pragma warning restore SA1123 // Do not place regions within elements

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

        [CommandOption("--output-path|-o")]
        [Description("Path to the output directory")]
        public DirectoryPath OutputDirectoryPath { get; set; } = null!;
    }
}