// <copyright file="ListOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    using System.IO;

    using NDepend.Path;

    public class ListOptions
    {
        public ListOptions(bool draft, bool published, DirectoryInfo siteTaxonomyDirectoryPath)
        {
            this.Draft = draft;
            this.Published = published;
            this.SiteTaxonomyDirectoryPath = siteTaxonomyDirectoryPath.FullName.ToAbsoluteDirectoryPath();
        }

        public bool Draft { get; }

        public bool Published { get; }

        public IAbsoluteDirectoryPath SiteTaxonomyDirectoryPath { get; }
    }
}