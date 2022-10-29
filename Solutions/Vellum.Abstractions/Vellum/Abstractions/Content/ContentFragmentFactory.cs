﻿// <copyright file="ContentFragmentFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Markdig;
    using Markdig.Extensions.Yaml;
    using Markdig.Renderers;
    using Markdig.Syntax;
    using NDepend.Path;
    using Vellum.Abstractions.Caching;
    using Vellum.Abstractions.Content.Formatting;
    using Vellum.Abstractions.Content.Parsers;
    using YamlDotNet.Serialization;

    public class ContentFragmentFactory
    {
        private readonly MarkdownPipeline pipeline;
        private readonly ContentFormatter contentFormatter;
        private readonly IDeserializer deserializer;

        public ContentFragmentFactory(ContentFormatter contentFormatter)
        {
            this.contentFormatter = contentFormatter;
            this.deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();
            this.pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseAutoIdentifiers()
                .UseYamlFrontMatter()
                .Build();
        }

        public ContentFragment Create(ContentBlock contentBlock, string content, IAbsoluteFilePath contentFragmentAbsoluteFilePath)
        {
            MarkdownDocument doc = Markdown.Parse(content, this.pipeline);

            string body = string.Empty;
            if (doc.HasContentOtherThanYamlFrontMatter())
            {
                body = this.RenderMarkdown(doc);
            }

            (string ContentType, PublicationStatus PublicationStatus, DateTime Date, Dictionary<string, dynamic> MetaData) result = this.ConvertFrontMatterToMetaData(doc, contentFragmentAbsoluteFilePath);

            return new ContentFragment
            {
                Body = body,
                ContentType = result.ContentType ?? contentBlock.ContentType,
                Date = result.Date,
                Hash = ContentHashing.Hash(content),
                Id = contentBlock.Id,
                MetaData = result.MetaData,
                PublicationStatus = result.PublicationStatus,
            };
        }

        private (string ContentType, PublicationStatus PublicationStatus, DateTime Date, Dictionary<string, dynamic> MetaData) ConvertFrontMatterToMetaData(
            MarkdownDocument markdown,
            IAbsoluteFilePath contentFragmentAbsoluteFilePath)
        {
            YamlFrontMatterBlock yamlBlock = markdown.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (yamlBlock == null)
            {
                return (string.Empty, PublicationStatus.Unknown, DateTime.MinValue, new Dictionary<string, dynamic> { { "FilePath", contentFragmentAbsoluteFilePath.ToString() } });
            }

            string yaml = string.Join(Environment.NewLine, yamlBlock.Lines.Lines.Select(l => l.ToString()).Where(x => !string.IsNullOrEmpty(x)));
            Dictionary<string, dynamic> frontMatter = this.deserializer.Deserialize<Dictionary<string, dynamic>>(yaml);

            frontMatter.Add("FilePath", contentFragmentAbsoluteFilePath.ToString());

            string contentType = string.Empty;

            if (frontMatter.TryGetValue("ContentType", out dynamic contentTypeDynamic))
            {
                contentType = contentTypeDynamic;
                frontMatter.Remove("ContentType");
            }

            PublicationStatus status = PublicationStatus.Unknown;
            if (frontMatter.TryGetValue("PublicationStatus", out dynamic statusDynamic))
            {
                status = PublicationStatusEnumParser.Parse(statusDynamic);
                frontMatter.Remove("PublicationStatus");
            }

            DateTime date = DateTime.MaxValue;

            if (frontMatter.TryGetValue("Date", out dynamic dateDynamic))
            {
                if (DateTime.TryParse(dateDynamic, out DateTime dateTime))
                {
                    date = dateTime;
                    frontMatter.Remove("Date");
                }
            }

            return (contentType, status, date, frontMatter);
        }

        private string RenderMarkdown(MarkdownDocument doc)
        {
            using var writer = new StringWriter();
            var renderer = new HtmlRenderer(writer);
            this.pipeline.Setup(renderer);
            renderer.Render(doc);

            return this.contentFormatter.Apply(html: writer.ToString());
        }
    }
}