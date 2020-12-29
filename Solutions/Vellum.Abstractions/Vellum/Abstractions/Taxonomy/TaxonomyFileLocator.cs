// <copyright file="TaxonomyFileLocator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.FileSystemGlobbing;
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

    using NDepend.Path;

    public class TaxonomyFileLocator
    {
        public IEnumerable<IAbsoluteFilePath> LocateRecursively(IAbsoluteDirectoryPath siteTaxonomyDirectoryPath)
        {
            var matcher = new Matcher();
            matcher.AddInclude("**/*.yml");

            PatternMatchingResult results = matcher.Execute(new DirectoryInfoWrapper(siteTaxonomyDirectoryPath.DirectoryInfo));

            return results.Files.Select(x => Path.GetFullPath(Path.Combine(siteTaxonomyDirectoryPath.ToString(), x.Path)).ToAbsoluteFilePath());
        }
    }
}
