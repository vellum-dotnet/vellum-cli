// <copyright file="TemplatePackage.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Templates
{
    using System.Collections.Generic;
    using System.IO;
    using NDepend.Path;
    using Newtonsoft.Json;
    using Vellum.Cli.Abstractions.Packages;

    public class TemplatePackage
    {
        public string PackageId { get; set; }

        public string Version { get; set; }

        public List<Template> Templates { get; } = new List<Template>();

        public string Id
        {
            get
            {
                return $"{this.PackageId}.{this.Version}";
            }
        }

        public string TemplateRepositoryPath { get; set; }

        public string InstalltionPath
        {
            get
            {
                return Path.Join(this.TemplateRepositoryPath, this.PackageId, this.Version);
            }
         }
    }
}