using Microsoft.Extensions.DependencyInjection;

using Spectre.IO;

using Vellum.Middleware.Abstractions;

namespace Vellum.Middleware;

/// <summary>
/// Extension methods for IVellumBuilder to register middleware.
/// </summary>
public static class VellumBuilderExtensions
{
    /// <summary>
    /// Adds file loading middleware to the Loading stage.
    /// </summary>
    /// <param name="builder">The Vellum builder.</param>
    /// <param name="siteTaxonomyDirectoryPath"></param>
    /// <returns>The builder for fluent configuration.</returns>
    public static IVellumBuilder UseSiteDetailsLoader(this IVellumBuilder builder, DirectoryPath siteTaxonomyDirectoryPath)
    {
        SiteDetailsLoaderMiddleware middleware = new(siteTaxonomyDirectoryPath);
        return builder.UseInStage(PipelineStage.Discovery, middleware.InvokeAsync);
    }

    public static IVellumBuilder UseSiteTaxonomyLoader(this IVellumBuilder builder, IServiceCollection services, DirectoryPath siteTaxonomyDirectoryPath)
    {
        SiteTaxonomyLoaderMiddleware middleware = new(services, siteTaxonomyDirectoryPath);
        return builder.UseInStage(PipelineStage.Loading, middleware.InvokeAsync);
    }

    /// <summary>
    /// Adds all standard middleware components in the correct 
    /// </summary>
    /// <param name="builder">The Vellum builder.</param>
    /// <param name="siteTaxonomyDirectoryPath">The directory path of the site taxonomy.</param>
    /// <param name="outputDirectory">The output directory.</param>
    /// <returns>The builder for fluent configuration.</returns>
    public static IVellumBuilder UseStandardMiddleware(this IVellumBuilder builder, IServiceCollection services, DirectoryPath siteTaxonomyDirectoryPath, DirectoryPath outputDirectory)
    {
        return builder
            .UseSiteDetailsLoader(siteTaxonomyDirectoryPath)
            .UseSiteTaxonomyLoader(services, siteTaxonomyDirectoryPath);
    }
}