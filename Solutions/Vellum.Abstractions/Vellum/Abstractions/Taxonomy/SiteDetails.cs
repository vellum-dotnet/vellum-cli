// <copyright file="SiteDetails.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics;

using Spectre.IO;

namespace Vellum.Abstractions.Taxonomy;

[DebuggerDisplay("{Url} | {Description}")]
public class SiteDetails : Representation
{
    public required string Description { get; set; }

    public required FilePath Path { get; set; }

    public required string Theme { get; set; }

    public required string Title { get; set; }

    public required string Url { get; set; }
}