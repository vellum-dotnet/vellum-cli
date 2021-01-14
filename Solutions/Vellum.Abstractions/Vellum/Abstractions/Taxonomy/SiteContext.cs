// <copyright file="SiteContext.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Collections.Generic;

    using Vellum.Abstractions.Content.Primitives;

    public class SiteContext
    {
        public bool Preview { get; set; }

        public NavigationNode Navigation { get; set; }

        public SiteDetails Details { get; set; }

        public IEnumerable<TaxonomyDocument> Pages { get; set; }
    }
}