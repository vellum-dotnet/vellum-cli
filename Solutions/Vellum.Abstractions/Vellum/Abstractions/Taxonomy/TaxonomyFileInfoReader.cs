// <copyright file="TaxonomyFileInfoReader.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NDepend.Path;

    using YamlDotNet.RepresentationModel;

    public class TaxonomyFileInfoReader
    {
        public async IAsyncEnumerable<TaxonomyFileInfo> ReadAsync(IEnumerable<IAbsoluteFilePath> files)
        {
            foreach (IAbsoluteFilePath file in files)
            {
                var yaml = new YamlStream();
                string contents = await File.ReadAllTextAsync(file.ToString());

                yaml.Load(new StringReader(contents));

                // Examine the stream
                var mapping = yaml.Documents[0].RootNode as YamlMappingNode;
                string contentType = ((YamlScalarNode)mapping.Children.FirstOrDefault(x => ((YamlScalarNode)x.Key).Value == "ContentType").Value).Value;

                yield return new TaxonomyFileInfo { ContentType = contentType, Path = file, Hash = ContentHashing.Hash(contents) };
            }
        }
    }
}
