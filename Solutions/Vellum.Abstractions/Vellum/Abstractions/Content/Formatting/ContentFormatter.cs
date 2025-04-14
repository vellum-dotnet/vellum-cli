// <copyright file="ContentFormatter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Vellum.Abstractions.Content.Formatting;

public class ContentFormatter : IContentFormatter
{
    private readonly IEnumerable<IContentTransform> transforms;

    public ContentFormatter(IEnumerable<IContentTransform> transforms)
    {
        this.transforms = transforms;
    }

    public string Apply(string html)
    {
        return this.transforms.Aggregate(html, (current, transform) => transform.Apply(current));
    }
}