// <copyright file="MarkdownWithYamlFrontMatterContentBlockParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using NDepend.Path;
using Vellum.Abstractions.Content.Formatting;
using Vellum.Abstractions.Taxonomy;

namespace Vellum.Abstractions.Content.Parsers;

public class MarkdownWithYamlFrontMatterContentBlockParser : IContentBlockParser
{
    private readonly IMemoryCache memoryCache;
    private readonly ContentFormatter contentFormatter;

    public MarkdownWithYamlFrontMatterContentBlockParser(IMemoryCache memoryCache, ContentFormatter contentFormatter)
    {
        this.memoryCache = memoryCache;
        this.contentFormatter = contentFormatter;
    }

    public string ContentType { get; protected set; }

    public async ValueTask<IEnumerable<ContentFragment>> ParseAsync(TaxonomyDocument taxonomyDocument, ContentBlock contentBlock)
    {
        var contentFragments = new List<ContentFragment>();

        foreach (IAbsoluteFilePath contentFragmentAbsoluteFilePath in FindContentFragmentFiles(contentBlock.Spec.Path, taxonomyDocument.Path.ParentDirectoryPath))
        {
            string cacheKey = $"{nameof(MarkdownWithYamlFrontMatterContentBlockParser)}::{contentFragmentAbsoluteFilePath}";

            if (!this.memoryCache.TryGetValue(cacheKey, out ContentFragment cachedContentFragment))
            {
                try
                {
                    string content = await File.ReadAllTextAsync(contentFragmentAbsoluteFilePath.ToString()).ConfigureAwait(false);

                    ContentFragment contentFragment = new MarkdownContentFragmentFactory(this.contentFormatter).Create(contentBlock, content, contentFragmentAbsoluteFilePath.ToString());

                    contentFragments.Add(contentFragment);

                    this.memoryCache.Set(cacheKey, contentFragment);
                }
                catch (Exception exception)
                {
                    throw new AggregateException(new InvalidOperationException($"Error parsing {contentFragmentAbsoluteFilePath}"), exception);
                }
            }
            else
            {
                contentFragments.Add(cachedContentFragment);
            }
        }

        // TODO: Content Block Filtering should be pulled out as the next stage in the processing pipeline.
        if (contentBlock.Spec.Tags.Count > 0)
        {
            contentFragments = contentFragments.Where(x => x.MetaData.ContainsKey("Tags"))
                .Where(t => ((List<object>)t.MetaData["Tags"])
                    .Intersect(contentBlock.Spec.Tags).Any()).ToList();
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

    private static IEnumerable<IAbsoluteFilePath> FindContentFragmentFiles(string contentFragmentPath, IAbsoluteDirectoryPath rootDirectory)
    {
        var matcher = new Matcher();

        matcher.AddInclude(contentFragmentPath);

        PatternMatchingResult matches = matcher.Execute(new DirectoryInfoWrapper(rootDirectory.DirectoryInfo));

        foreach (FilePatternMatch match in matches.Files)
        {
            yield return match.Path.ToRelativeFilePath().GetAbsolutePathFrom(rootDirectory);
        }
    }
}