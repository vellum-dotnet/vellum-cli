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
    public static IVellumBuilder UseContentFragmentParsingLoader(this IVellumBuilder builder, IServiceCollection services)
    {
        ContentFragmentParsingMiddleware middleware = new(services);
        return builder.UseInStage(PipelineStage.PreParsing, middleware.InvokeAsync);
    }

    public static IVellumBuilder UseSiteContextLoader(this IVellumBuilder builder)
    {
        SiteContextLoaderMiddleware middleware = new();
        return builder.UseInStage(PipelineStage.Parsing, middleware.InvokeAsync);
    }

    /// <summary>
    /// Adds frontmatter validation middleware to the PostParsing stage. Every parsed content fragment is
    /// checked against the interfaces its content type and extensions map to, and all coercion failures
    /// are logged and published to stage data under
    /// <see cref="ContentFragmentValidationMiddleware.ValidationFailuresKey"/>.
    /// </summary>
    /// <param name="builder">The Vellum builder.</param>
    /// <param name="services">The service collection the middleware resolves its dependencies from.</param>
    /// <returns>The builder for fluent configuration.</returns>
    public static IVellumBuilder UseContentFragmentValidation(this IVellumBuilder builder, IServiceCollection services)
    {
        ContentFragmentValidationMiddleware middleware = new(services);
        return builder.UseInStage(PipelineStage.PostParsing, middleware.InvokeAsync);
    }

    /// <summary>
    /// Adds all standard middleware components in the correct order.
    /// </summary>
    /// <param name="builder">The Vellum builder.</param>
    /// <param name="services">The service collection the middleware components resolve their dependencies from.</param>
    /// <param name="siteTaxonomyDirectoryPath">The directory path of the site taxonomy.</param>
    /// <param name="outputDirectory">The output directory.</param>
    /// <returns>The builder for fluent configuration.</returns>
    public static IVellumBuilder UseStandardMiddleware(this IVellumBuilder builder, IServiceCollection services, DirectoryPath siteTaxonomyDirectoryPath, DirectoryPath outputDirectory)
    {
        return builder
            .UseSiteDetailsLoader(siteTaxonomyDirectoryPath)
            .UseSiteTaxonomyLoader(services, siteTaxonomyDirectoryPath)
            .UseContentFragmentParsingLoader(services)
            .UseContentFragmentValidation(services)
            .UseSiteContextLoader();
    }
}