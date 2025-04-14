// <copyright file="TaxonomyFileInfo.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics;

using NDepend.Path;

namespace Vellum.Abstractions.Taxonomy;

[DebuggerDisplay("{Path}")]
public class TaxonomyFileInfo : Representation
{
    public string Hash { get; set; }

    public IAbsoluteFilePath Path { get; set; }
}