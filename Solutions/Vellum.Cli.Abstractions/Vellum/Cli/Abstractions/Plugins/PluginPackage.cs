// <copyright file="PluginPackage.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Spectre.IO;
using Vellum.Cli.Abstractions.Packages;

namespace Vellum.Cli.Abstractions.Plugins;

public class PluginPackage
{
    public string Name { get; set; }

    public string Version { get; set; }

    public DirectoryPath RepositoryPath { get; set; }

    public List<PackageDetail> Details { get; } = [];

    [JsonIgnore]
    public List<string> Plugins { get; } = [];

    public string Id
    {
        get
        {
            return $"{this.Name}.{this.Version}";
        }
    }

    public DirectoryPath PluginPath
    {
        get
        {
            return System.IO.Path.Combine(this.RepositoryPath.ToString(), this.Name, this.Version);
        }
    }
}