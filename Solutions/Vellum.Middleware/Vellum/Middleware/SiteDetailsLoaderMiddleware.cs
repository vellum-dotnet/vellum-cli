using Spectre.IO;
using Vellum.Abstractions.Taxonomy;
using Vellum.Middleware.Abstractions;

namespace Vellum.Middleware;

/// <summary>
/// Middleware that loads content from files in the ContentFiles collection.
/// </summary>
public class SiteDetailsLoaderMiddleware
{
    private readonly DirectoryPath siteTaxonomyDirectoryPath;

    /// <summary>
    /// Initializes a new instance of the SiteDetailsLoaderMiddleware class.
    /// </summary>
    public SiteDetailsLoaderMiddleware(DirectoryPath siteTaxonomyDirectoryPath)
    {
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
            SiteDetailsRepository siteDetailsRepository = new();
            SiteDetails? siteDetails = await siteDetailsRepository.FindAsync(this.siteTaxonomyDirectoryPath);

            VellumContextBuilder builder = VellumContextBuilder.From(context);

            if (siteDetails is not null)
            {
                builder.AddSiteDetails(siteDetails);
            }

            VellumContext updatedContext = builder.Build();

            await next(updatedContext);
        };
    }
}