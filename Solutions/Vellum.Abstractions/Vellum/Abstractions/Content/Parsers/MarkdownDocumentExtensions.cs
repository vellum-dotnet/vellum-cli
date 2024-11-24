// <copyright file="MarkdownDocumentExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Parsers
{
    using System.Linq;
    using Markdig.Syntax;

    public static class MarkdownDocumentExtensions
    {
        public static bool HasContentOtherThanYamlFrontMatter(this MarkdownDocument document)
        {
            return document.Descendants<ParagraphBlock>().Any();
        }
    }
}