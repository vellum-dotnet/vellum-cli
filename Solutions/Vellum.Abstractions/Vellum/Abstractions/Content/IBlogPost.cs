// <copyright file="IBlogPost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

public interface IBlogPost : IContent, IExtensible, IArticle, IPublishable, ICategorizable, ITaggable, IFaqable
{
    Dates? Dates { get; set; }

    string? HeaderImageUrl { get; set; }
}