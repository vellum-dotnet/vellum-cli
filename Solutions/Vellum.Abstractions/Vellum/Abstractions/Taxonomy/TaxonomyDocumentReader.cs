// <copyright file="TaxonomyDocumentReader.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading.Tasks;

using Spectre.IO;
using Vellum.Abstractions.Content.Primitives;
using Vellum.Abstractions.IO;
using Vellum.Abstractions.Parsers;

namespace Vellum.Abstractions.Taxonomy;

public class TaxonomyDocumentReader : IFileReader<TaxonomyDocument>
{
    public const string RegisteredContentType = WellKnown.Taxonomies.ContentTypes.Page;

    public string ContentType => RegisteredContentType;

    public async Task<TaxonomyDocument> ReadAsync(FilePath filePath)
    {
        TaxonomyDocument template = await new YamlParser<TaxonomyDocument>().ParseAsync(filePath).ConfigureAwait(false);
        template.Path = filePath;

        // set default (visible / enabled) if missing
        template.Navigation!.Footer ??= new NavigationOption
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