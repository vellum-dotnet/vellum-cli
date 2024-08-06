// <copyright file="TaxonomyDocument.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Taxonomy
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using NDepend.Path;

    using Vellum.Abstractions.Content;
    using Vellum.Abstractions.Content.Primitives;

    [DebuggerDisplay("{Navigation.Url}")]
    public class TaxonomyDocument : Representation, ITaxonomyDocument
    {
        public List<ContentFragment> ContentFragments { get; set; } = new List<ContentFragment>();

        public string Title { get; set; }

        public string Template { get; set; }

        public IAbsoluteFilePath Path { get; set; }

        public string Hash { get; set; }

        public PageMetaData MetaData { get; set; }

        public OpenGraph OpenGraph { get; set; }

        public Navigation Navigation { get; set; }

        public IEnumerable<ContentBlock> ContentBlocks { get; set; } = Enumerable.Empty<ContentBlock>();

        public string FileUrl
        {
            get
            {
                string fileName = string.Empty;

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
}
