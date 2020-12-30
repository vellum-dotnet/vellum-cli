// <copyright file="SiteTaxonomyRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Threading.Tasks;
    using NDepend.Path;
    using Vellum.Abstractions;
    using Vellum.Abstractions.Parsers;

    public class SiteTaxonomyRepository
    {
        public async Task<SiteTaxonomy> FindAsync(IAbsoluteDirectoryPath siteTaxonomyDirectoryPath)
        {
            SiteTaxonomy siteTaxonomy = null;

            await foreach (TaxonomyFileInfo file in new TaxonomyFileInfoRepository().FindAllAsync(siteTaxonomyDirectoryPath))
            {
                if (file.ContentType == WellKnown.ContentTypes.Site)
                {
                    siteTaxonomy = await new YamlParser<SiteTaxonomy>().ParseAsync(file.Path).ConfigureAwait(false);
                    siteTaxonomy.Path = file.Path;

                    break;
                }
            }

            return siteTaxonomy;
        }
    }
}
