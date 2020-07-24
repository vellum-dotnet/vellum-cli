// <copyright file="ICommandPluginHost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Plugins
{
    using System.Collections.Generic;
    using System.CommandLine;
    using NDepend.Path;

    public interface ICommandPluginHost
    {
        IEnumerable<Command> Discover(IEnumerable<IAbsoluteDirectoryPath> pluginPaths);
    }
}