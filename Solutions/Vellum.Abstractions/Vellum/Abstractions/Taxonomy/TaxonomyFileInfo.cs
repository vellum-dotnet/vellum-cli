// <copyright file="TaxonomyFileInfo.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics;

using Spectre.IO;

namespace Vellum.Abstractions.Taxonomy;

[DebuggerDisplay("{Path}")]
public class TaxonomyFileInfo : Representation
{
    public required string Hash { get; set; }

    public required FilePath Path { get; set; }
}