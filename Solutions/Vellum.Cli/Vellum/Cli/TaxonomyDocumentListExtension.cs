// <copyright file="TaxonomyDocumentListExtension.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

using Microsoft.Extensions.DependencyInjection;

using Vellum.Abstractions;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Taxonomy;

using static Vellum.Abstractions.WellKnown;

namespace Vellum.Cli;

public static class TaxonomyDocumentListExtension
{
    private static ServiceProvider? sp;

    public static void Configure(ServiceProvider serviceProvider)
    {
        sp = serviceProvider;
    }

    public static List<ContentFragment> GetAllAuthorsAsContentFragments(this List<TaxonomyDocument> taxonomyDocuments)
    {
        return taxonomyDocuments.SelectMany(x => x.ContentFragments.Where(y => y.ContentType == WellKnown.ContentFragments.ContentTypes.Authors)).Distinct().ToList();
    }

    public static List<ContentFragment> GetAllBlogPostsAsContentFragments(this List<TaxonomyDocument> taxonomyDocuments)
    {
        return taxonomyDocuments.SelectMany(x => x.ContentFragments.Where(y => y.ContentType == WellKnown.ContentFragments.ContentTypes.BlogMarkdown)).OrderBy(x => x.Date).Distinct().ToList();
    }

    public static List<ContentFragment> GetAllBlogPostsWithAuthorsAsContentFragments(this List<TaxonomyDocument> taxonomyDocuments)
    {
        List<ContentFragment> authors = GetAllAuthorsAsContentFragments(taxonomyDocuments);
        List<ContentFragment> blogPosts = GetAllBlogPostsAsContentFragments(taxonomyDocuments);

        foreach (ContentFragment blogPost in blogPosts)
        {
            blogPost.MetaData.TryGetValue("AuthorId", out dynamic? authorId);
            if (authorId is not null)
            {
                // The AuthorId value is stored in author.MetaData["AuthorId"] so we need to find the author with the matching AuthorId
                ContentFragment? matchingAuthor = authors.FirstOrDefault(x =>
                {
                    if (x.MetaData.TryGetValue("AuthorId", out dynamic? author))
                    {
                        if (string.Compare(author, authorId, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            return true;
                        }
                    }

                    return false;
                });

                if (matchingAuthor != null)
                {
                    blogPost.MetaData["Author"] = matchingAuthor;
                }
            }
        }

        return blogPosts;
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
        return ConvertAll<IBlogPost>(GetAllBlogPostsWithAuthorsAsContentFragments(taxonomyDocuments)).OrderBy(x => x.Date).ToList();
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

    public static List<T> ConvertAll<T>(this List<ContentFragment> contentFragments)
        where T : class, IContent
    {
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