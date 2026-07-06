// <copyright file="TaxonomyDocumentRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Spectre.IO;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.Parsers;
using Vellum.Abstractions.IO;

namespace Vellum.Abstractions.Taxonomy;

public class TaxonomyDocumentRepository
{
    private readonly IServiceCollection services;

    public TaxonomyDocumentRepository(IServiceCollection services)
    {
        this.services = services;
    }

    public async IAsyncEnumerable<TaxonomyDocument> LoadAllAsync(DirectoryPath siteTaxonomyDirectoryPath)
    {
        ServiceProvider serviceProvider = this.services.BuildServiceProvider();

        TaxonomyFileInfoRepository taxonomyFileInfoRepository = new();

        await foreach (TaxonomyFileInfo file in taxonomyFileInfoRepository.FindAllAsync(siteTaxonomyDirectoryPath))
        {
            IFileReader<TaxonomyDocument>? reader = serviceProvider.GetContent<IFileReader<TaxonomyDocument>>(file.ContentType);

            if (reader is not null)
            {
                yield return await reader.ReadAsync(file).ConfigureAwait(false);
            }
        }
    }

    public async IAsyncEnumerable<TaxonomyDocument> LoadContentFragmentsAsync(IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments)
    {
        ServiceProvider serviceProvider = this.services.BuildServiceProvider();

        await foreach (TaxonomyDocument taxonomyDocument in taxonomyDocuments)
        {
            foreach (ContentBlock contentBlock in taxonomyDocument.ContentBlocks)
            {
                IContentBlockParser? contentBlockParser = serviceProvider.GetContent<IContentBlockParser>(contentBlock.ContentType);

                if (contentBlockParser is not null)
                {
                    IEnumerable<ContentFragment> results = await contentBlockParser.ParseAsync(taxonomyDocument, contentBlock).ConfigureAwait(false);

                    if (!string.IsNullOrEmpty(contentBlock.Spec?.ContentType))
                    {
                        taxonomyDocument.ContentFragments.AddRange(results.Where(x => x.ContentType == contentBlock.Spec.ContentType));
                    }
                    else
                    {
                        taxonomyDocument.ContentFragments.AddRange(results);
                    }
                }
            }

            yield return taxonomyDocument;
        }

        MemoryCache mc = (MemoryCache)serviceProvider.GetRequiredService<IMemoryCache>();

        mc.Compact(100);
    }
}