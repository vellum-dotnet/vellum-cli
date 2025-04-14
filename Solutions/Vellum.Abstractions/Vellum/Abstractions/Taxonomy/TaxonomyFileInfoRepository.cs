// <copyright file="TaxonomyFileInfoRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using NDepend.Path;

namespace Vellum.Abstractions.Taxonomy;

public class TaxonomyFileInfoRepository
{
    public async IAsyncEnumerable<TaxonomyFileInfo> FindAllAsync(IAbsoluteDirectoryPath siteTaxonomyDirectoryPath)
    {
        var siteTaxonomyLocator = new TaxonomyFileLocator();
        var taxonomyFileParser = new TaxonomyFileInfoReader();

        IEnumerable<IAbsoluteFilePath> files = siteTaxonomyLocator.LocateRecursively(siteTaxonomyDirectoryPath);

        await foreach (TaxonomyFileInfo taxonomyFileInfo in taxonomyFileParser.ReadAsync(files))
        {
            yield return taxonomyFileInfo;
        }
    }
}