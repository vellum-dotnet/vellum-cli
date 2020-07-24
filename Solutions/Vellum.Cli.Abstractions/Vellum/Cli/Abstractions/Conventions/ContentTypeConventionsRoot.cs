// <copyright file="ContentTypeConventionsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Conventions
{
    using System.Collections.Generic;
    using NDepend.Path;

    public class ContentTypeConventionsRoot
    {
        public IAbsoluteFilePath FilePath { get; set; }

        public List<ContentTypeConvention> Conventions { get; set; }
    }
}