// <copyright file="ContentTypeInterfaceRegistryExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vellum.Abstractions.Content.Extensions;

namespace Vellum.Abstractions.Content.ContentFactories;

/// <summary>
/// Registration extensions for the content-type-to-interface registry.
/// </summary>
public static class ContentTypeInterfaceRegistryExtensions
{
    /// <summary>
    /// Maps a content type to the interface that content fragments of that type are duck-typed onto.
    /// Registering the same content type again overrides the earlier mapping.
    /// </summary>
    /// <typeparam name="TInterface">The interface that content fragments of the given content type are duck-typed onto.</typeparam>
    /// <param name="serviceCollection">The service collection to register the mapping with.</param>
    /// <param name="contentType">The content type, as declared in YAML frontmatter <c>ContentType</c> or <c>Extensions</c>.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddContentTypeInterface<TInterface>(this IServiceCollection serviceCollection, string contentType)
        where TInterface : class
    {
        serviceCollection.AddSingleton(new ContentTypeInterfaceRegistration(contentType, typeof(TInterface)));

        return serviceCollection;
    }

    /// <summary>
    /// Registers the content-type-to-interface mappings for Vellum's well-known content types.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the mappings with.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddWellKnownContentTypeInterfaces(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddContentTypeInterface<IBlogPost>(WellKnown.ContentFragments.ContentTypes.BlogMarkdown);
        serviceCollection.AddContentTypeInterface<ISeries>(WellKnown.ContentFragments.ContentTypes.Series);
        serviceCollection.AddContentTypeInterface<IPromotions>(WellKnown.ContentFragments.ContentTypes.Promotion);

        return serviceCollection;
    }

    /// <summary>
    /// Registers the content extensibility pipeline: the content-type-to-interface registry and the
    /// factory that duck-types extended content fragments onto their declared extension interfaces.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the pipeline with.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddVellumContentExtensibility(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IContentTypeInterfaceFactory, ContentTypeInterfaceRegistry>();
        serviceCollection.TryAddSingleton<IExtensionTypeFactory, ExtensionTypeFactory>();
        serviceCollection.AddWellKnownContentTypeInterfaces();

        return serviceCollection;
    }
}