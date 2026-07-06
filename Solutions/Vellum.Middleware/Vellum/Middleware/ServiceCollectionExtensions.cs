// <copyright file="ServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

using Vellum.Abstractions.Content.Transformations;
using Vellum.Middleware.Transformations;

namespace Vellum.Middleware;

/// <summary>
/// Extension methods for registering content fragment transformers with the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the AuthorIdResolver transformer with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for fluent configuration.</returns>
    public static IServiceCollection AddAuthorIdResolver(this IServiceCollection services)
    {
        services.AddTransient<IContentFragmentTransformer, AuthorIdResolver>();
        return services;
    }

    /// <summary>
    /// Registers all standard content fragment transformers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for fluent configuration.</returns>
    public static IServiceCollection AddContentFragmentTransformers(this IServiceCollection services)
    {
        // Add all standard transformers here
        services.AddAuthorIdResolver();

        // Add additional transformers as they are created
        // services.AddTransient<IContentFragmentTransformer, YourNextTransformer>();

        return services;
    }
}