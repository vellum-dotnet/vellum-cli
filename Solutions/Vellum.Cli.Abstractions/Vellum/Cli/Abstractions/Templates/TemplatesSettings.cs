// <copyright file="TemplatesSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Cli.Abstractions.Templates;

public class TemplatesSettings
{
    public DefaultTemplate DefaultTemplate { get; set; }

    public List<TemplatePackage> Packages { get; set; } = [];
}