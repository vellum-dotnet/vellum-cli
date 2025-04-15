// <copyright file="TaxonomyDocumentListExtension.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Vellum.Abstractions;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Taxonomy;

public static class TaxonomyDocumentListExtension
{
    private static ServiceProvider? sp;

    public static void Configure(ServiceProvider serviceProvider)
    {
        sp = serviceProvider;
    }

    public static IAuthor Author(this IBlogPost blogPost, List<IAuthor> authors)
    {
        return authors.First(x => string.Compare(x.AuthorId, blogPost.AuthorId, StringComparison.InvariantCultureIgnoreCase) == 0);
    }

    public static List<IAuthor> GetAllAuthors(this List<TaxonomyDocument> taxonomyDocuments)
    {
        return GetAll<IAuthor>(taxonomyDocuments, WellKnown.ContentFragments.ContentTypes.Authors);
    }

    public static List<IBlogPost> GetAllBlogPosts(this List<TaxonomyDocument> taxonomyDocuments)
    {
        return GetAll<IBlogPost>(taxonomyDocuments, WellKnown.ContentFragments.ContentTypes.BlogMarkdown).OrderBy(x => x.Date).ToList();
    }

    public static List<T> GetAll<T>(this List<TaxonomyDocument> taxonomyDocuments, string contentType)
        where T : class, IContent
    {
        List<ContentFragment> contentFragments = taxonomyDocuments.SelectMany(x => x.ContentFragments.Where(y => y.ContentType == contentType)).Distinct().ToList();
        List<T> dataType = [];

        foreach (ContentFragment contentFragment in contentFragments)
        {
            ContentFragmentTypeFactory<T>? contentFragmentTypeFactory = sp?.GetContent<ContentFragmentTypeFactory<T>>(contentFragment.ContentType.AsContentFragmentFactory());
            T? item = contentFragmentTypeFactory?.Create(contentFragment);
            if (item is not null)
            {
                dataType.Add(item);
            }
        }

        return dataType;
    }
}