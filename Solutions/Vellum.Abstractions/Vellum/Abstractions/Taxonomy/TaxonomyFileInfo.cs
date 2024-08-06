// <copyright file="TaxonomyFileInfo.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Diagnostics;

    using NDepend.Path;

    [DebuggerDisplay("{Path}")]
    public class TaxonomyFileInfo : Representation
    {
        public string Hash { get; set; }

        public IAbsoluteFilePath Path { get; set; }
    }
}