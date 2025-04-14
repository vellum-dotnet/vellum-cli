// <copyright file="IContentBlockParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Vellum.Abstractions.Taxonomy;

namespace Vellum.Abstractions.Content.Parsers;

public interface IContentBlockParser
{
    ValueTask<IEnumerable<ContentFragment>> ParseAsync(TaxonomyDocument taxonomyDocument, ContentBlock contentBlock);
}