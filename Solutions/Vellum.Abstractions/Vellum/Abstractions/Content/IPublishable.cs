// <copyright file="IPublishable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

public interface IPublishable
{
    string BaseUrl { get; set; }

    string Slug { get; set; }

    string Url { get; set; }
}