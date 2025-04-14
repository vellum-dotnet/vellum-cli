// <copyright file="MarkdownDocumentExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Linq;
using Markdig.Syntax;

namespace Vellum.Abstractions.Content.Parsers;

public static class MarkdownDocumentExtensions
{
    public static bool HasContentOtherThanYamlFrontMatter(this MarkdownDocument document)
    {
        return document.Descendants<ParagraphBlock>().Any();
    }
}