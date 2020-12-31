// <copyright file="TaxonomyDocumentRespository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;

    using NDepend.Path;
    using Vellum.Abstractions.Content;
    using Vellum.Abstractions.Content.ContentFactories;
    using Vellum.Abstractions.Content.Parsers;
    using Vellum.Abstractions.IO;

    public class TaxonomyDocumentRespository
    {
        private readonly IServiceCollection services;

        public TaxonomyDocumentRespository(IServiceCollection services)
        {
            this.services = services;
        }

        public async IAsyncEnumerable<TaxonomyDocument> LoadAllAsync(IAbsoluteDirectoryPath siteTaxonomyDirectoryPath)
        {
            this.services.AddWellKnownTaxonomyContentTypes();
            ServiceProvider serviceProvider = this.services.BuildServiceProvider();

            var taxonomyFileInfoRepository = new TaxonomyFileInfoRepository();

            IAsyncEnumerable<TaxonomyFileInfo> files = taxonomyFileInfoRepository.FindAllAsync(siteTaxonomyDirectoryPath);

            await foreach (TaxonomyFileInfo file in files)
            {
                IFileReader<TaxonomyDocument> reader = serviceProvider.GetContent<IFileReader<TaxonomyDocument>>(file.ContentType);

                if (reader != null)
                {
                    TaxonomyDocument taxonomyDocument = await reader.ReadAsync(file.Path).ConfigureAwait(false);
                    taxonomyDocument.Hash = file.Hash;

                    yield return taxonomyDocument;
                }
                else
                {
                    // console.Error.Write($"Cannot Read file with ContentType {file.ContentType}" + System.Environment.NewLine);
                }
            }
        }

        public async IAsyncEnumerable<TaxonomyDocument> LoadContentFragmentsAsync(IAsyncEnumerable<TaxonomyDocument> taxonomyDocuments)
        {
            this.services.AddWellKnownContentBlockContentTypes();
            ServiceProvider serviceProvider = this.services.BuildServiceProvider();

            // throw new InvalidOperationException($"There is no ContentBlockParser registered for ContentType {contentBlock.ContentType}");
            await foreach (TaxonomyDocument taxonomyDocument in taxonomyDocuments)
            {
                taxonomyDocument.ContentBlocks ??= new List<ContentBlock>();

                foreach (ContentBlock contentBlock in taxonomyDocument.ContentBlocks)
                {
                    IContentBlockParser contentBlockParser = serviceProvider.GetContent<IContentBlockParser>(contentBlock.ContentType);

                    if (contentBlockParser != null)
                    {
                        IEnumerable<ContentFragment> results = await contentBlockParser.ParseAsync(taxonomyDocument, contentBlock).ConfigureAwait(false);

                        if (!string.IsNullOrEmpty(contentBlock.Spec.ContentType))
                        {
                            IEnumerable<ContentFragment> filteredResults = results.Where(x => x.ContentType == contentBlock.Spec.ContentType);
                            taxonomyDocument.ContentFragments.AddRange(filteredResults);
                        }
                        else
                        {
                            taxonomyDocument.ContentFragments.AddRange(results);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"There is no ContentBlockParser registered for ContentType {contentBlock.ContentType}");
                    }
                }

                yield return taxonomyDocument;
            }

            var mc = (MemoryCache)serviceProvider.GetRequiredService<IMemoryCache>();

            mc.Compact(100);
        }
    }
}
