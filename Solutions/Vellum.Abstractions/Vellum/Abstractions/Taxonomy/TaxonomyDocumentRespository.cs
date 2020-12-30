// <copyright file="TaxonomyDocumentRespository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    using NDepend.Path;

    using Vellum.Abstractions.Content.ContentFactories;
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
    }
}
