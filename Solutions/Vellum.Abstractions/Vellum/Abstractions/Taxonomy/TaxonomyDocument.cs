// <copyright file="TaxonomyDocument.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using NDepend.Path;

    using Vellum.Abstractions.Content;
    using Vellum.Abstractions.Content.Primitives;

    [DebuggerDisplay("{Navigation.Url}")]
    public class TaxonomyDocument : Representation
    {
        public string Title { get; set; }

        public string Template { get; set; }

        public IAbsoluteFilePath Path { get; set; }

        public string Hash { get; set; }

        public PageMetaData MetaData { get; set; }

        public OpenGraph OpenGraph { get; set; }

        public Navigation Navigation { get; set; }

        public IEnumerable<ContentBlock> ContentBlocks { get; set; } = Enumerable.Empty<ContentBlock>();
    }
}
