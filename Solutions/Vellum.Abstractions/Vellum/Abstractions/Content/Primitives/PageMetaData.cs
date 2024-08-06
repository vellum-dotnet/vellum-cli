// <copyright file="PageMetaData.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Primitives
{
    using System.Collections.Generic;

    public class PageMetaData
    {
        public string CanonicalUrl { get; set; }

        public string Description { get; set; }

        public List<string> Keywords { get; set; } = new List<string>();
    }
}
