// <copyright file="TaxonomyDocumentReader.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Threading.Tasks;

    using NDepend.Path;

    using Vellum.Abstractions.Content.Primitives;
    using Vellum.Abstractions.IO;
    using Vellum.Abstractions.Parsers;

    public class TaxonomyDocumentReader : IFileReader<TaxonomyDocument>
    {
        public const string RegisteredContentType = WellKnown.ContentTypes.Page;

        public string ContentType => RegisteredContentType;

        public async Task<TaxonomyDocument> ReadAsync(IAbsoluteFilePath filePath)
        {
            TaxonomyDocument template = await new YamlParser<TaxonomyDocument>().ParseAsync(filePath).ConfigureAwait(false);
            template.Path = filePath;

            // set default (visible / enabled) if missing
            template.Navigation.Footer ??= new NavigationOption
            {
                Link = true,
                Visible = true,
            };

            template.Navigation.Header ??= new NavigationOption
            {
                Link = true,
                Visible = true,
            };

            return template;
        }
    }
}
