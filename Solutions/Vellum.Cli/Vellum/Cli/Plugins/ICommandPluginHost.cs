// <copyright file="ICommandPluginHost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Plugins
{
    using System.Collections.Generic;
    using System.CommandLine;
    using Spectre.IO;
    using Vellum.Cli.Abstractions.Commands;

    public interface ICommandPluginHost
    {
        List<ICommandPlugin> Discover(IEnumerable<DirectoryPath> pluginPaths);
    }
}