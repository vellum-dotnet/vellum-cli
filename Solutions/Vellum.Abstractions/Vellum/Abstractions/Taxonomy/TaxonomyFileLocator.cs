// <copyright file="TaxonomyFileLocator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

using Spectre.IO;

namespace Vellum.Abstractions.Taxonomy;

public class TaxonomyFileLocator
{
    public IEnumerable<FilePath> LocateRecursively(DirectoryPath siteTaxonomyDirectoryPath)
    {
        var matcher = new Matcher();
        matcher.AddInclude("**/*.yml");

        PatternMatchingResult results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(siteTaxonomyDirectoryPath.FullPath)));

        return results.Files.Select(x => siteTaxonomyDirectoryPath.CombineWithFilePath(x.Path));
    }
}