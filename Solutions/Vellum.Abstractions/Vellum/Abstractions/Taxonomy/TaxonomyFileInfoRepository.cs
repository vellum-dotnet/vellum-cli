// <copyright file="TaxonomyFileInfoRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Spectre.IO;

namespace Vellum.Abstractions.Taxonomy;

public class TaxonomyFileInfoRepository
{
    public async IAsyncEnumerable<TaxonomyFileInfo> FindAllAsync(DirectoryPath siteTaxonomyDirectoryPath)
    {
        TaxonomyFileLocator siteTaxonomyLocator = new();
        TaxonomyFileInfoReader taxonomyFileParser = new();

        IEnumerable<FilePath> files = siteTaxonomyLocator.LocateRecursively(siteTaxonomyDirectoryPath);

        await foreach (TaxonomyFileInfo taxonomyFileInfo in taxonomyFileParser.ReadAsync(files))
        {
            yield return taxonomyFileInfo;
        }
    }
}