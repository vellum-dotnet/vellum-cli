// <copyright file="NavigationNode.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Primitives
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Url}")]
    public class NavigationNode
    {
        public string Description { get; set; }

        public string Id
        {
            get
            {
                return this.Url.ToString().TrimStart('/').Replace("/", "-").Replace(".html", string.Empty).ToLowerInvariant();
            }
        }

        public NavigationOption Header { get; set; }

        public NavigationOption Footer { get; set; }

        public Url Url { get; set; }

        public int Rank { get; set; }

        public string Title { get; set; }

        public List<NavigationNode> Children { get; set; } = new List<NavigationNode>();
    }
}