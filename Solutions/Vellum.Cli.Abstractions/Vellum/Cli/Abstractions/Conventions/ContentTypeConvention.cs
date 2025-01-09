// <copyright file="ContentTypeConvention.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Cli.Abstractions.Conventions;

public class ContentTypeConvention
{
    public string ContentType { get; set; }

    public string TemplatePath { get; set; }

    public List<Convention> Conventions { get; set; }
}