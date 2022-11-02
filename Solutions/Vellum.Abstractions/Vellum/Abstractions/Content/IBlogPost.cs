// <copyright file="IBlogPost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System.Collections.Generic;

#pragma warning disable CS8632

    public interface IBlogPost : IContent, ISeries, IArticle, IPublishable, ICategorizable, ITaggable, IFaqable
    {
        Dates? Dates { get; set; }

        string? HeaderImageUrl { get; set; }
    }

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

}