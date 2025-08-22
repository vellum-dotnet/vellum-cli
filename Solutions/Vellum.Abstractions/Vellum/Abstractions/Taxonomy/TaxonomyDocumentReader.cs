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

    public async Task<TaxonomyDocument> ReadAsync(TaxonomyFileInfo file)
    {
        TaxonomyDocument template = await new YamlParser<TaxonomyDocument>().ParseAsync(file.Path).ConfigureAwait(false);

        template.ContentBlocks ??= [];
        template.ContentFragments ??= [];
        template.Path = file.Path;
        template.Hash = file.Hash;

        // set default (visible / enabled) if missing
        template.Navigation!.Footer ??= new NavigationOption
        {
            Link = false,
            Visible = false,
        };

        template.Navigation.Header ??= new NavigationOption
        {
            Link = false,
            Visible = false,
        };

        return template;
    }
}