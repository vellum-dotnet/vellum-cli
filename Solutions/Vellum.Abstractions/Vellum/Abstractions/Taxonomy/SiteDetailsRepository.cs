// <copyright file="SiteDetailsRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Spectre.IO;
using Vellum.Abstractions.Parsers;

namespace Vellum.Abstractions.Taxonomy;

public class SiteDetailsRepository
{
    public async Task<SiteDetails?> FindAsync(DirectoryPath siteTaxonomyDirectoryPath)
    {
        SiteDetails? siteDetails = null;
        TaxonomyFileInfoRepository taxonomyFileInfoRepository = new();

        await foreach (TaxonomyFileInfo file in taxonomyFileInfoRepository.FindAllAsync(siteTaxonomyDirectoryPath))
        {
            if (file.ContentType != WellKnown.Taxonomies.ContentTypes.Site)
            {
                continue;
            }

            siteDetails = await new YamlParser<SiteDetails>().ParseAsync(file.Path).ConfigureAwait(false);
            siteDetails.Path = file.Path;

            break;
        }

        return siteDetails;
    }
}