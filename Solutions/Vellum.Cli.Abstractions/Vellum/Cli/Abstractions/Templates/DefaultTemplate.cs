// <copyright file="DefaultTemplate.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Templates
{
    using NDepend.Path;

    public class DefaultTemplate
    {
        public string ContentType { get; set; }

        public string PackageName { get; set; }

        public string PackagePath { get; set; }
    }
}