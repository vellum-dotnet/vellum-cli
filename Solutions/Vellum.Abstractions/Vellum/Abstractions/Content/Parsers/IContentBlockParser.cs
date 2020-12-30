// <copyright file="IContentBlockParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Parsers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Vellum.Abstractions.Taxonomy;

    public interface IContentBlockParser
    {
        Task<IEnumerable<ContentFragment>> ParseAsync(TaxonomyDocument taxonomyDocument, ContentBlock contentBlock);
    }
}