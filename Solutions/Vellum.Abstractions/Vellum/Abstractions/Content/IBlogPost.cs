// <copyright file="IBlogPost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System.Collections.Generic;

#pragma warning disable CS8632

    public interface IBlogPost : IContent
    {
        IEnumerable<string>? Attachments { get; set; }

        IAuthor? Author { get; set; }

        string? BaseUrl { get; set; }

        string? Body { get; set; }

        IEnumerable<string>? Category { get; set; }

        Dates? Dates { get; set; }

        string? Excerpt { get; set; }

        IEnumerable<Faq>? Faqs { get; set; }

        string? HeaderImageUrl { get; set; }

        bool? IsSeries { get; }

        string? PartTitle { get; set; }

        int? Position { get; set; }

        string? Profile { get; set; }

        string? Series { get; set; }

        string? Slug { get; set; }

        IEnumerable<string>? Tags { get; set; }

        string? Title { get; set; }

        string? Url { get; set; }

        string? UserName { get; }
    }

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

}