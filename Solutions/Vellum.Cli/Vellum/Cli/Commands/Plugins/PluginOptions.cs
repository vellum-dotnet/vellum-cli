// <copyright file="PluginOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins
{
    using System.IO;

    public class PluginOptions
    {
        public PluginOptions(string packageId)
        {
            this.PackageId = packageId;
        }

        public string PackageId { get; }
    }
}