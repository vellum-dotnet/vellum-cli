// <copyright file="TaxonomyFileInfoReader.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Spectre.IO;
using Vellum.Abstractions.Caching;
using YamlDotNet.RepresentationModel;

namespace Vellum.Abstractions.Taxonomy;

public class TaxonomyFileInfoReader
{
    public async IAsyncEnumerable<TaxonomyFileInfo> ReadAsync(IEnumerable<FilePath> files)
    {
        foreach (FilePath file in files)
        {
            YamlStream yaml = [];
            string contents = await File.ReadAllTextAsync(file.ToString()!);

            yaml.Load(new StringReader(contents));

            if (yaml.Documents[0].RootNode is YamlMappingNode mapping)
            {
                string? contentType = ((YamlScalarNode)mapping.Children.FirstOrDefault(x => ((YamlScalarNode)x.Key).Value == "ContentType").Value).Value ?? string.Empty;

                yield return new TaxonomyFileInfo { ContentType = contentType, Path = file, Hash = ContentHashing.Hash(contents) };
            }
        }
    }
}