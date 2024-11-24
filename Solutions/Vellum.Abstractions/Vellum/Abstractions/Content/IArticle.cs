// <copyright file="IArticle.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

#pragma warning disable CS8632
public interface IArticle : IAuthorId
{
    string Body { get; set; }

    string Excerpt { get; set; }

    string Title { get; set; }
}