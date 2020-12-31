// <copyright file="SiteDetailsRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Threading.Tasks;
    using NDepend.Path;
    using Vellum.Abstractions;
    using Vellum.Abstractions.Parsers;

    public class SiteDetailsRepository
    {
        public async Task<SiteDetails> FindAsync(IAbsoluteDirectoryPath siteTaxonomyDirectoryPath)
        {
            SiteDetails siteDetails = null;

            await foreach (TaxonomyFileInfo file in new TaxonomyFileInfoRepository().FindAllAsync(siteTaxonomyDirectoryPath))
            {
                if (file.ContentType == WellKnown.Taxonomies.ContentTypes.Site)
                {
                    siteDetails = await new YamlParser<SiteDetails>().ParseAsync(file.Path).ConfigureAwait(false);
                    siteDetails.Path = file.Path;

                    break;
                }
            }

            return siteDetails;
        }
    }
}
