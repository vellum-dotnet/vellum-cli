// <copyright file="Template.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Templates
{
    using NDepend.Path;

    public class Template
    {
        public string ContentType { get; set; }

        public string NestedFilePath { get; set; }
    }
}