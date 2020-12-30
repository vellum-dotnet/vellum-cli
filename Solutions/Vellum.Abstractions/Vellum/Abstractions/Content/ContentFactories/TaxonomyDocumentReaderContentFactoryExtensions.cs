// <copyright file="TaxonomyDocumentReaderContentFactoryExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.ContentFactories
{
    using Corvus.ContentHandling;

    using Microsoft.Extensions.DependencyInjection;

    using Vellum.Abstractions.Taxonomy;

    public static class TaxonomyDocumentReaderContentFactoryExtensions
    {
        /// <summary>
        /// Add content management content to the container.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which to add the content.</param>
        /// <returns>The service collection wth the content added.</returns>
        public static IServiceCollection AddWellKnownTaxonomyContentTypes(this IServiceCollection serviceCollection)
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
            factory.RegisterTransientContent<TaxonomyDocumentReader>(WellKnown.Taxonomies.ContentTypes.Blog.Index);
            factory.RegisterTransientContent<TaxonomyDocumentReader>(WellKnown.Taxonomies.ContentTypes.Blog.Post);
            factory.RegisterTransientContent<TaxonomyDocumentReader>(WellKnown.Taxonomies.ContentTypes.Blog.PostsByAuthor);
            factory.RegisterTransientContent<TaxonomyDocumentReader>(WellKnown.Taxonomies.ContentTypes.Blog.PostsByEdition);
            factory.RegisterTransientContent<TaxonomyDocumentReader>(WellKnown.Taxonomies.ContentTypes.HomePage);
            factory.RegisterTransientContent<TaxonomyDocumentReader>(WellKnown.Taxonomies.ContentTypes.Page);

            return factory;
        }
    }
}