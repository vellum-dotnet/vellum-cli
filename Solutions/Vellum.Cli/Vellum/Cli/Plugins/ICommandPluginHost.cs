// <copyright file="ICommandPluginHost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

using Spectre.IO;

using Vellum.Cli.Abstractions.Commands;

namespace Vellum.Cli.Plugins;

public interface ICommandPluginHost
{
    IEnumerable<ICommandPlugin> Discover(IEnumerable<DirectoryPath> pluginPaths);
}