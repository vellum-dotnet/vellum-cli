using Microsoft.Extensions.DependencyInjection;

using Spectre.IO;

using Vellum.Abstractions.Taxonomy;
using Vellum.Middleware.Abstractions;

namespace Vellum.Middleware;

/// <summary>
/// Middleware that loads content from files in the ContentFiles collection.
/// </summary>
public class SiteTaxonomyLoaderMiddleware
{
    private readonly IServiceCollection services;
    private readonly DirectoryPath siteTaxonomyDirectoryPath;

    /// <summary>
    /// Initializes a new instance of the SiteTaxonomyLoaderMiddleware class.
    /// </summary>
    public SiteTaxonomyLoaderMiddleware(IServiceCollection services, DirectoryPath siteTaxonomyDirectoryPath)
    {
        this.services = services;
        this.siteTaxonomyDirectoryPath = siteTaxonomyDirectoryPath;
    }

    /// <summary>
    /// Creates a middleware delegate for the pipeline.
    /// </summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>The middleware delegate.</returns>
    public RequestDelegate InvokeAsync(RequestDelegate next)
    {
        return async context =>
        {
            TaxonomyDocumentRepository taxonomyDocumentRepository = new(this.services);

            IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments = taxonomyDocumentRepository.LoadAllAsync(this.siteTaxonomyDirectoryPath);
            List<TaxonomyDocument> loaded = await taxonomyDocumentRepository.LoadContentFragmentsAsync(taxonomyDocuments).ToListAsync();

            VellumContextBuilder builder = VellumContextBuilder.From(context);

            if (loaded.Count > 0)
            {
                builder.AddTaxonomyDocuments(loaded);
            }

            VellumContext updatedContext = builder.Build();

            await next(updatedContext);
        };
    }
}