// <copyright file="ContentTypeConvention.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Configuration
{
    using System.Collections.Generic;

    public class ContentTypeConvention
    {
        public string ContentType { get; set; }

        public List<Convention> Conventions { get; set; }
    }
}