// <copyright file="SiteDetails.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Diagnostics;

    using NDepend.Path;

    [DebuggerDisplay("{Url} | {Description}")]
    public class SiteDetails : Representation
    {
        public string Description { get; set; }

        public IAbsoluteFilePath Path { get; set; }

        public string Theme { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}