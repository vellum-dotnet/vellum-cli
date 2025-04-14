// <copyright file="MarkdownContentFragmentFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Syntax;
using Vellum.Abstractions.Caching;
using Vellum.Abstractions.Content.Extensions;
using Vellum.Abstractions.Content.Formatting;
using Vellum.Abstractions.Content.Parsers;
using YamlDotNet.Serialization;

namespace Vellum.Abstractions.Content;

public class MarkdownContentFragmentFactory
{
    private readonly MarkdownPipeline pipeline;
    private readonly IContentFormatter contentFormatter;
    private readonly IDeserializer deserializer;

    public MarkdownContentFragmentFactory(IContentFormatter contentFormatter)
    {
        this.contentFormatter = contentFormatter;
        this.deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();
        this.pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseAutoIdentifiers()
            .UseYamlFrontMatter()
            .Build();
    }

    public ContentFragment Create(ContentBlock contentBlock, string content, string contentFragmentAbsoluteFilePath)
    {
        MarkdownDocument doc = Markdown.Parse(content, this.pipeline);

        string body = string.Empty;
        if (doc.HasContentOtherThanYamlFrontMatter())
        {
            body = this.RenderMarkdown(doc);
        }

        (string ContentType, PublicationStatus PublicationStatus, DateTime Date, IEnumerable<string> Extensions, Dictionary<string, dynamic> MetaData) result = this.ConvertFrontMatterToMetaData(doc, contentFragmentAbsoluteFilePath);

        return new ContentFragment
        {
            Body = body,
            ContentType = result.ContentType ?? contentBlock.ContentType,
            Date = result.Date,
            Extensions = result.Extensions,
            Hash = ContentHashing.Hash(content.NormalizeLineEndings()),
            Id = contentBlock.Id,
            MetaData = result.MetaData,
            PublicationStatus = result.PublicationStatus,
        };
    }

    private (string ContentType, PublicationStatus PublicationStatus, DateTime Date, IEnumerable<string> Extensions, Dictionary<string, dynamic> MetaData) ConvertFrontMatterToMetaData(MarkdownDocument markdown, string contentFragmentAbsoluteFilePath)
    {
        YamlFrontMatterBlock? yamlBlock = markdown.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        if (yamlBlock == null)
        {
            return (string.Empty, PublicationStatus.Unknown, DateTime.MinValue, [], new Dictionary<string, dynamic> { { "FilePath", contentFragmentAbsoluteFilePath } });
        }

        string yaml = string.Join(Environment.NewLine, yamlBlock.Lines.Lines.Select(l => l.ToString()).Where(x => !string.IsNullOrEmpty(x)));
        Dictionary<string, dynamic> frontMatter = this.deserializer.Deserialize<Dictionary<string, dynamic>>(yaml);

        frontMatter.Add("FilePath", contentFragmentAbsoluteFilePath);

        string contentType = string.Empty;
        IEnumerable<string> extensions = [];

        if (frontMatter.TryGetValue("ContentType", out dynamic? contentTypeDynamic))
        {
            contentType = contentTypeDynamic;
            frontMatter.Remove("ContentType");
        }

        if (frontMatter.TryGetValue("Extensions", out dynamic? extensionsDynamic))
        {
            if (extensionsDynamic is IEnumerable<object> objects)
            {
                extensions = objects.Cast<string>();
            }

            frontMatter.Remove("Extensions");
        }

        PublicationStatus status = PublicationStatus.Unknown;
        if (frontMatter.TryGetValue("PublicationStatus", out dynamic? statusDynamic))
        {
            status = PublicationStatusEnumParser.Parse(statusDynamic);
            frontMatter.Remove("PublicationStatus");
        }

        DateTime date = DateTime.MaxValue;

        if (frontMatter.TryGetValue("Date", out dynamic? dateDynamic))
        {
            if (DateTime.TryParse(dateDynamic, out DateTime dateTime))
            {
                date = dateTime;
                frontMatter.Remove("Date");
            }
        }

        return (contentType, status, date, extensions, frontMatter);
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