// <copyright file="ITaxonomyDocument.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using NDepend.Path;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.Primitives;

namespace Vellum.Abstractions.Taxonomy;

public interface ITaxonomyDocument
{
    string Title { get; set; }

    string Template { get; set; }

    IAbsoluteFilePath Path { get; set; }

    string Hash { get; set; }

    PageMetaData MetaData { get; set; }

    OpenGraph OpenGraph { get; set; }

    Navigation Navigation { get; set; }

    IEnumerable<ContentBlock> ContentBlocks { get; set; }

    string ContentType { get; set; }
}