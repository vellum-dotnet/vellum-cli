// <copyright file="ContentBlockContentFactoryExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Corvus.ContentHandling;

using Microsoft.Extensions.DependencyInjection;
using Vellum.Abstractions.Content.Parsers;

namespace Vellum.Abstractions.Content.ContentFactories;

public static class ContentBlockContentFactoryExtensions
{
    /// <summary>
    /// Add content management content to the container.
    /// </summary>
    /// <param name="serviceCollection">The service collection to which to add the content.</param>
    /// <returns>The service collection wth the content added.</returns>
    public static IServiceCollection AddWellKnownContentBlockContentTypes(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddContent(factory => factory.RegisterContent());
    }

    /// <summary>
    /// Registers the content management content types with the factory.
    /// </summary>
    /// <param name="factory">The factory with which to register the content.</param>
    /// <returns>The factory with the content registered.</returns>
    private static ContentFactory RegisterContent(this ContentFactory factory)
    {
        factory.RegisterTransientContent<MarkdownWithYamlFrontMatterContentBlockParser>(WellKnown.ContentFragments.ContentTypes.Authors);
        factory.RegisterTransientContent<MarkdownWithYamlFrontMatterContentBlockParser>(WellKnown.ContentFragments.ContentTypes.BlogMarkdown);
        factory.RegisterTransientContent<MarkdownWithYamlFrontMatterContentBlockParser>(WellKnown.ContentFragments.ContentTypes.ContentMarkdown);
        factory.RegisterTransientContent<MarkdownWithYamlFrontMatterContentBlockParser>(WellKnown.ContentFragments.ContentTypes.Hero);
        factory.RegisterTransientContent<MarkdownWithYamlFrontMatterContentBlockParser>(WellKnown.ContentFragments.ContentTypes.ImageHero);

        return factory;
    }
}