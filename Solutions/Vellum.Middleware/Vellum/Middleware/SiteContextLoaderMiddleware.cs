using Vellum.Abstractions.Content.Primitives;
using Vellum.Abstractions.Taxonomy;
using Vellum.Middleware.Abstractions;

namespace Vellum.Middleware;

public class SiteContextLoaderMiddleware
{
    /// <summary>
    /// Creates a middleware delegate for the pipeline.
    /// </summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>The middleware delegate.</returns>
    public RequestDelegate InvokeAsync(RequestDelegate next)
    {
        return async context =>
        {
            VellumContextBuilder builder = VellumContextBuilder.From(context);

            SiteTaxonomyParser siteTaxonomyParser = new();
            NavigationNode siteNavigation = siteTaxonomyParser.Parse(context.TaxonomyDocuments);

            SiteContext siteContext = new()
            {
                Preview = false,
                Navigation = siteNavigation,
                Pages = new Dictionary<string, object>(),
                Details = context.SiteDetails!,
            };

            builder.AddSiteContext(siteContext);

            VellumContext updatedContext = builder.Build();

            await next(updatedContext);
        };
    }
}