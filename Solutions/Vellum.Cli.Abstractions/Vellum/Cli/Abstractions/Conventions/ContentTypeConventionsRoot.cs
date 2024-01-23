// <copyright file="ContentTypeConventionsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Spectre.IO;

namespace Vellum.Cli.Abstractions.Conventions;

public class ContentTypeConventionsRoot
{
    public FilePath FilePath { get; set; }

    public List<ContentTypeConvention> Conventions { get; set; }
}