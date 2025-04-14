// <copyright file="PageMetaData.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content.Primitives;

public class PageMetaData
{
    public string? CanonicalUrl { get; set; }

    public string? Description { get; set; }

    public List<string> Keywords { get; set; } = [];
}