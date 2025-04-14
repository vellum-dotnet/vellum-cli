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
using Spectre.IO;
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

    public string ContentType { get; set; } = string.Empty;

    public async ValueTask<IEnumerable<ContentFragment>> ParseAsync(TaxonomyDocument taxonomyDocument, ContentBlock contentBlock)
    {
        List<ContentFragment> contentFragments = [];

        foreach (FilePath contentFragmentAbsoluteFilePath in FindContentFragmentFiles(contentBlock.Spec?.Path!, taxonomyDocument.Path.GetDirectory()))
        {
            string cacheKey = $"{nameof(MarkdownWithYamlFrontMatterContentBlockParser)}::{contentFragmentAbsoluteFilePath}";

            if (!this.memoryCache.TryGetValue(cacheKey, out ContentFragment? cachedContentFragment))
            {
                try
                {
                    if (!FileSystem.Shared.File.Exists(contentFragmentAbsoluteFilePath))
                    {
                        throw new FileNotFoundException($"The content fragment file {contentFragmentAbsoluteFilePath} does not exist.");
                    }

                    string content = await File.ReadAllTextAsync(contentFragmentAbsoluteFilePath.ToString()!).ConfigureAwait(false);

                    ContentFragment contentFragment = new MarkdownContentFragmentFactory(this.contentFormatter).Create(contentBlock, content, contentFragmentAbsoluteFilePath.ToString()!);

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
                contentFragments.Add(cachedContentFragment!);
            }
        }

        // TODO: Content Block Filtering should be pulled out as the next stage in the processing pipeline.
        if (contentBlock.Spec?.Tags?.Count > 0)
        {
            contentFragments = contentFragments.Where(x => x.MetaData.ContainsKey("Tags"))
                .Where(t => ((List<object>)t.MetaData["Tags"])
                    .Intersect(contentBlock.Spec.Tags).Any()).ToList();
        }

        if (contentBlock.Spec is { Count: > 0 } && contentBlock.Spec?.Count.HasValue == true)
        {
            contentFragments = contentFragments.Take(contentBlock.Spec.Count.Value).ToList();
        }

        for (int i = 0; i < contentFragments.Count; i++)
        {
            contentFragments[i].Position = i;
        }

        return contentFragments;
    }

    private static IEnumerable<FilePath> FindContentFragmentFiles(string contentFragmentPath, DirectoryPath rootDirectory)
    {
        var matcher = new Matcher();

        matcher.AddInclude(contentFragmentPath);

        PatternMatchingResult matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(rootDirectory.FullPath)));

        foreach (FilePatternMatch match in matches.Files)
        {
            yield return new FilePath(match.Path).MakeAbsolute(rootDirectory);
        }
    }
}