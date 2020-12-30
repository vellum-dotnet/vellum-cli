// <copyright file="ContentFormatter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public class ContentFormatter
    {
        private readonly IServiceProvider serviceProvider;

        public ContentFormatter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string Apply(string html)
        {
            IEnumerable<IContentTransform> transforms = this.serviceProvider.GetServices<IContentTransform>();
            return transforms.Aggregate(html, (current, transform) => transform.Apply(current));
        }
    }
}