// <copyright file="SiteDetails.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics;

using NDepend.Path;

namespace Vellum.Abstractions.Taxonomy;

[DebuggerDisplay("{Url} | {Description}")]
public class SiteDetails : Representation
{
    public string Description { get; set; }

    public IAbsoluteFilePath Path { get; set; }

    public string Theme { get; set; }

    public string Title { get; set; }

    public string Url { get; set; }
}