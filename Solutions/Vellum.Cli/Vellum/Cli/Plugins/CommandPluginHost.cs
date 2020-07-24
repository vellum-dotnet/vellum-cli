// <copyright file="CommandPluginHost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.Linq;
    using McMaster.NETCore.Plugins;
    using NDepend.Path;
    using Vellum.Cli.Abstractions.Commands;

    public class CommandPluginHost : ICommandPluginHost
    {
        public IEnumerable<Command> Discover(IEnumerable<IAbsoluteDirectoryPath> pluginPaths)
        {
            var loaders = new List<PluginLoader>();

            foreach (IAbsoluteDirectoryPath dir in pluginPaths)
            {
                foreach (IAbsoluteFilePath child in dir.ChildrenFilesPath.Where(x => x.FileExtension == ".dll"))
                {
                    // Enable Hot Reload so that we can delete plugins, otherwise you get an access denied exception
                    var pluginLoader = PluginLoader.CreateFromAssemblyFile(
                        child.FileInfo.FullName,
                        sharedTypes: new[] { typeof(ICommandPlugin) },
                        config => config.EnableHotReload = true);

                    loaders.Add(pluginLoader);
                }
            }

            foreach (PluginLoader loader in loaders)
            {
                foreach (Type pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(ICommandPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    // This assumes the implementation of IPlugin has a parameterless constructor
                    var plugin = Activator.CreateInstance(pluginType) as ICommandPlugin;

                    yield return plugin?.Command();
                }
            }
        }
    }
}