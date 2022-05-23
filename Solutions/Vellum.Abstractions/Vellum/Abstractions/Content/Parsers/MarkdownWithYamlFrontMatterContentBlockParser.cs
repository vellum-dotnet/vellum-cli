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
    using Markdig.Renderers;
    using Markdig.Syntax;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.FileSystemGlobbing;
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

    using NDepend.Path;

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
            this.contentFormatter = contentFormatter;
            this.deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();
            this.memoryCache = memoryCache;
            this.pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseAutoIdentifiers()
                .UseYamlFrontMatter()
                .Build();
        }

        public string ContentType { get; protected set; }

        public async Task<IEnumerable<ContentFragment>> ParseAsync(TaxonomyDocument taxonomyDocument, ContentBlock contentBlock)
        {
            var contentFragments = new List<ContentFragment>();

            foreach (IAbsoluteFilePath contentFragmentAbsoluteFilePath in this.FindContentFragmentFiles(contentBlock.Spec.Path, taxonomyDocument.Path.ParentDirectoryPath))
            {
                string cacheKey = $"{nameof(MarkdownWithYamlFrontMatterContentBlockParser)}::{contentFragmentAbsoluteFilePath}";

                if (!this.memoryCache.TryGetValue(cacheKey, out ContentFragment cachedContentFragment))
                {
                    string content = await File.ReadAllTextAsync(contentFragmentAbsoluteFilePath.ToString()).ConfigureAwait(false);

                    ContentFragment contentFragment = ContentFragmentFactory.Create(contentBlock, content);
                    MarkdownDocument doc = Markdown.Parse(content, this.pipeline);

                    try
                    {
                        contentFragment.MetaData = this.ConvertFrontMatterToMetaData(doc, contentFragmentAbsoluteFilePath, contentFragment);

                        if (!string.IsNullOrEmpty(contentBlock.Spec.ContentType) &&
                            !string.IsNullOrEmpty(contentFragment.ContentType) &&
                            contentBlock.Spec.ContentType != contentFragment.ContentType)
                        {
                            continue;
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new AggregateException(new InvalidOperationException($"Error parsing {contentFragmentAbsoluteFilePath}"), exception);
                    }

                    if (doc.HasContentOtherThanYamlFrontMatter())
                    {
                        contentFragment.Body = this.RenderMarkdown(doc);
                    }

                    contentFragments.Add(contentFragment);

                    this.memoryCache.Set(cacheKey, contentFragment);
                }
                else
                {
                    contentFragments.Add(cachedContentFragment);
                }
            }

            // TODO: Content Block Filtering should be pulled out as the next stage in the processing pipeline.
            if (contentBlock.Spec.Tags.Count > 0)
            {
                contentFragments = contentFragments.Where(x => x.MetaData.ContainsKey("Tags")).Where(t => ((List<object>)t.MetaData["Tags"]).Intersect(contentBlock.Spec.Tags).Any()).ToList();
            }

            if (contentBlock.Spec.Count > 0)
            {
                contentFragments = contentFragments.Take(contentBlock.Spec.Count).ToList();
            }

            for (int i = 0; i < contentFragments.Count; i++)
            {
                contentFragments[i].Position = i;
            }

            return contentFragments;
        }

        private Dictionary<string, dynamic> ConvertFrontMatterToMetaData(MarkdownDocument markdown, IAbsoluteFilePath contentFragmentAbsoluteFilePath, ContentFragment cf)
        {
            YamlFrontMatterBlock yamlBlock = markdown.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (yamlBlock == null)
            {
                return new Dictionary<string, dynamic> { { "FilePath", contentFragmentAbsoluteFilePath.ToString() } };
            }

            string yaml = string.Join(Environment.NewLine, yamlBlock.Lines.Lines.Select(l => l.ToString()).Where(x => !string.IsNullOrEmpty(x)));
            Dictionary<string, dynamic> frontMatter = this.deserializer.Deserialize<Dictionary<string, dynamic>>(yaml);

            frontMatter.Add("FilePath", contentFragmentAbsoluteFilePath.ToString());

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

            return frontMatter;
        }

        private IEnumerable<IAbsoluteFilePath> FindContentFragmentFiles(string contentFragmentPath, IAbsoluteDirectoryPath rootDirectory)
        {
            var matcher = new Matcher();

            matcher.AddInclude(contentFragmentPath);

            PatternMatchingResult matches = matcher.Execute(new DirectoryInfoWrapper(rootDirectory.DirectoryInfo));

            foreach (FilePatternMatch match in matches.Files)
            {
                yield return match.Path.ToRelativeFilePath().GetAbsolutePathFrom(rootDirectory);
            }
        }

        private string RenderMarkdown(MarkdownDocument doc)
        {
            using (var writer = new StringWriter())
            {
                var renderer = new HtmlRenderer(writer);
                this.pipeline.Setup(renderer);
                renderer.Render(doc);

                return this.contentFormatter.Apply(html: writer.ToString());
            }
        }
    }
}