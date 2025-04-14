// <copyright file="TaxonomyDocument.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Spectre.IO;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.Primitives;

namespace Vellum.Abstractions.Taxonomy;

[DebuggerDisplay("{Navigation.Url}")]
public class TaxonomyDocument : Representation, ITaxonomyDocument
{
    public List<ContentFragment> ContentFragments { get; set; } = [];

    public required string Title { get; set; }

    public required string Template { get; set; }

    public required FilePath Path { get; set; }

    public required string Hash { get; set; }

    public required PageMetaData MetaData { get; set; }

    public required OpenGraph OpenGraph { get; set; }

    public required Navigation Navigation { get; set; }

    public IEnumerable<ContentBlock> ContentBlocks { get; set; } = [];

    public string FileUrl
    {
        get
        {
            string fileName = string.Empty;

            if (this.Navigation is null)
            {
                return fileName;
            }

            if (this.Navigation.Url.ToString().EndsWith("/") || string.IsNullOrEmpty(System.IO.Path.GetExtension(this.Navigation.Url.ToString())))
            {
                fileName = "index.html";
            }

            return Flurl.Url.Combine(this.Navigation.Url.ToString(), fileName);
        }
    }

    public string TemplatePath
    {
        get
        {
            if (!string.IsNullOrEmpty(this.Template))
            {
                return Flurl.Url.Combine(this.Template).TrimStart('/');
            }

            string fileName;

            if (this.Navigation is null)
            {
                return string.Empty;
            }

            if (this.Navigation.Url.ToString().EndsWith("/") || string.IsNullOrEmpty(System.IO.Path.GetExtension(this.Navigation.Url.ToString())))
            {
                fileName = "index";
            }
            else
            {
                return Flurl.Url.Combine(System.IO.Path.ChangeExtension(this.Navigation.Url.ToString(), string.Empty)).TrimStart('/').TrimEnd('.');
            }

            return Flurl.Url.Combine(this.Navigation.Url.ToString(), fileName).TrimStart('/');
        }
    }
}