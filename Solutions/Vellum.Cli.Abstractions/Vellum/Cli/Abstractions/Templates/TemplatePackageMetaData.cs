// <copyright file="TemplatePackageMetaData.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Templates
{
    using System.Collections.Generic;
    using System.IO;
    using NDepend.Path;
    using Newtonsoft.Json;
    using Vellum.Cli.Abstractions.Packages;

    public class TemplatePackageMetaData
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public IAbsoluteDirectoryPath RepositoryPath { get; set; }

        public List<PackageDetail> Details { get; } = new List<PackageDetail>();

        [JsonIgnore]
        public List<string> Templates { get; } = new List<string>();

        public string Id
        {
            get
            {
                return $"{this.Name}.{this.Version}";
            }
        }

        public IAbsoluteDirectoryPath TemplatePath
        {
            get
            {
                return Path.Combine(this.RepositoryPath.ToString(), this.Version).ToAbsoluteDirectoryPath();
            }
        }
    }
}