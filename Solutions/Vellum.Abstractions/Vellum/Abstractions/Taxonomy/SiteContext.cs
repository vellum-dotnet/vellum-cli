// <copyright file="SiteContext.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

using Vellum.Abstractions.Content.Primitives;

namespace Vellum.Abstractions.Taxonomy;

public class SiteContext
{
    public bool Preview { get; set; }

    public required NavigationNode Navigation { get; set; }

    public required SiteDetails Details { get; set; }

    public required Dictionary<string, object> Pages { get; set; }
}