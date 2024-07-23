// <copyright file="TemplatePackage.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;

namespace Vellum.Cli.Abstractions.Templates;

public class TemplatePackage
{
    public string PackageId { get; set; }

    public string Version { get; set; }

    public List<Template> Templates { get; } = [];

    public string Id
    {
        get
        {
            return $"{this.PackageId}.{this.Version}";
        }
    }

    public string TemplateRepositoryPath { get; set; }

    public string InstallationPath
    {
        get
        {
            return Path.Join(this.TemplateRepositoryPath, this.PackageId, this.Version);
        }
    }
}