﻿// <copyright file="CommandPluginHost.cs" company="Endjin Limited">
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
            var assemblies = pluginPaths.SelectMany(dir => dir.ChildrenFilesPath.Where(x => x.FileExtension == ".dll")).DistinctBy(x => x.FileName).ToList();

            foreach (IAbsoluteFilePath assembly in assemblies)
            {
                // Enable Hot Reload so that we can delete plugins, otherwise you get an access denied exception
                var pluginLoader = PluginLoader.CreateFromAssemblyFile(
                    assembly.FileInfo.FullName,
                    sharedTypes: new[] { typeof(ICommandPlugin) },
                    config => config.EnableHotReload = true);

                loaders.Add(pluginLoader);
            }

            foreach (PluginLoader loader in loaders)
            {
                foreach (Type pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(ICommandPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    ICommandPlugin plugin = null;

                    try
                    {
                        // This assumes the implementation of IPlugin has a parameterless constructor
                        plugin = Activator.CreateInstance(pluginType) as ICommandPlugin;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }

                    if (plugin != null)
                    {
                        yield return plugin?.Command();
                    }
                }
            }
        }
    }
}