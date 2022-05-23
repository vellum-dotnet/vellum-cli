// <copyright file="TemplatesSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Templates
{
    using System.Collections.Generic;

    public class TemplatesSettings
    {
        public DefaultTemplate DefaultTemplate { get; set; }

        public List<TemplatePackage> Packages { get; set; } = new List<TemplatePackage>();
    }
}