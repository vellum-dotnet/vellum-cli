// <copyright file="MarkdownWithYamlFrontMatterContentBlockParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Markdig;
    using Markdig.Extensions.Yaml;
    using Markdig.Syntax;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.FileSystemGlobbing;
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
    using Vellum.Abstractions.Caching;
    using Vellum.Abstractions.Content.Formatting;
    using Vellum.Abstractions.Taxonomy;
    using YamlDotNet.Serialization;

    public class MarkdownWithYamlFrontMatterContentBlockParser : IContentBlockParser
    {
        private readonly IMemoryCache memoryCache;
        private readonly ContentFormatter contentFormatter;
        private readonly IDeserializer deserializer;
        private readonly MarkdownPipeline pipeline;

        public MarkdownWithYamlFrontMatterContentBlockParser(IMemoryCache memoryCache, ContentFormatter contentFormatter)
        {
            this.memoryCache = memoryCache;
            this.contentFormatter = contentFormatter;
            this.deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();
            this.pipeline = new MarkdownPipelineBuilder()
                .UseAutoIdentifiers()
                .UseGridTables()
                .UsePipeTables()
                .UseYamlFrontMatter()
                .Build();
        }

        public string ContentType { get; protected set; }

        public async Task<IEnumerable<ContentFragment>> ParseAsync(TaxonomyDocument taxonomyDocument, ContentBlock contentBlock)
        {
            var cfs = new List<ContentFragment>();
            var matcher = new Matcher();
            FileInfo templateFileInfo = taxonomyDocument.Path.FileInfo;

            matcher.AddInclude(contentBlock.Spec.Path);

            PatternMatchingResult results = matcher.Execute(new DirectoryInfoWrapper(templateFileInfo.Directory));

            foreach (var file in results.Files)
            {
                string cacheKey = $"MarkdownWithYamlFrontMatterContentBlockParser::{file.Path}";

                if (!this.memoryCache.TryGetValue<ContentFragment>(cacheKey, out ContentFragment cachedContentFragment))
                {
                    string filePath = Path.GetFullPath(Path.Join(templateFileInfo.Directory?.FullName, file.Path));
                    string content = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);

                    var cf = new ContentFragment
                    {
                        ContentType = contentBlock.ContentType,
                        Hash = ContentHashing.Hash(content),
                        Id = contentBlock.Id,
                    };

                    MarkdownDocument doc = Markdown.Parse(content, this.pipeline);
                    YamlFrontMatterBlock yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

                    if (yamlBlock != null)
                    {
                        string yaml = string.Join(Environment.NewLine, yamlBlock.Lines.Lines.Select(l => l.ToString()).Where(x => !string.IsNullOrEmpty(x)));

                        try
                        {
                            Dictionary<string, dynamic> frontMatter = this.deserializer.Deserialize<Dictionary<string, dynamic>>(yaml);

                            frontMatter.Add("FilePath", filePath);

                            if (frontMatter.TryGetValue("ContentType", out dynamic contentType))
                            {
                                cf.ContentType = contentType;
                                frontMatter.Remove("ContentType");
                            }

                            if (frontMatter.TryGetValue("PublicationStatus", out dynamic status))
                            {
                                cf.PublicationStatus = PublicationStatusEnumParser.Parse(status);
                                frontMatter.Remove("PublicationStatus");
                            }

                            if (frontMatter.TryGetValue("Date", out dynamic date))
                            {
                                if (DateTime.TryParse(date, out DateTime dateTime))
                                {
                                    cf.Date = dateTime;
                                    frontMatter.Remove("Date");
                                }
                            }

                            cf.MetaData = frontMatter;

                            if (!string.IsNullOrEmpty(contentBlock.Spec.ContentType) &&
                                !string.IsNullOrEmpty(cf.ContentType) &&
                                contentBlock.Spec.ContentType != cf.ContentType)
                            {
                                continue;
                            }
                        }
                        catch (Exception exception)
                        {
                            throw new AggregateException(new InvalidOperationException($"Error parsing {filePath}"), exception);
                        }
                    }
                    else
                    {
                        var frontMatter = new Dictionary<string, dynamic> { { "FilePath", filePath } };
                        cf.MetaData = frontMatter;
                    }

                    cf.Body = this.contentFormatter.Apply(Markdown.ToHtml(content, this.pipeline));
                    cfs.Add(cf);

                    this.memoryCache.Set(cacheKey, cf);
                }
                else
                {
                    cfs.Add(cachedContentFragment);
                }
            }

            // TODO: Content Block Filtering should be pulled out as the next stage in the processing pipeline.
            if (contentBlock.Spec.Tags.Count > 0)
            {
                cfs = cfs.Where(x => x.MetaData.ContainsKey("Tags")).Where(t =>
                    ((List<object>)t.MetaData["Tags"]).Intersect(contentBlock.Spec.Tags).Any()).ToList();
            }

            if (contentBlock.Spec.Count > 0)
            {
                cfs = cfs.Take(contentBlock.Spec.Count).ToList();
            }

            for (int i = 0; i < cfs.Count; i++)
            {
                cfs[i].Position = i;
            }

            return cfs;
        }
    }
}