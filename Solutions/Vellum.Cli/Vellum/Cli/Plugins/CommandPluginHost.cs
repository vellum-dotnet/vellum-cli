// <copyright file="CommandPluginHost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.IO;
    using System.Linq;
    using McMaster.NETCore.Plugins;
    using Microsoft.Extensions.FileSystemGlobbing;
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
    using Spectre.IO;
    using Vellum.Cli.Abstractions.Commands;

    public class CommandPluginHost : ICommandPluginHost
    {
        public bool RaiseExceptions { get; set; }

        public List<ICommandPlugin> Discover(IEnumerable<DirectoryPath> pluginPaths)
        {
            List<FileInfo> files = [];
            List<PluginLoader> loaders = [];
            Matcher matcher = new();
            matcher.AddInclude("*.dll");

            DirectoryInfo commonDir = new(AppContext.BaseDirectory);

            foreach (DirectoryPath directoryPath in pluginPaths)
            {
                string result = System.IO.Path.GetRelativePath(directoryPath.FullPath, commonDir.FullName);
                string relPath = System.IO.Path.Combine(commonDir.FullName, result, directoryPath.FullPath);
                string fullPath = System.IO.Path.GetFullPath(relPath);

                PatternMatchingResult matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(fullPath)));

                foreach (FilePatternMatch file in matches.Files)
                {
                    files.Add(new FileInfo(System.IO.Path.Join(fullPath, file.Path)));
                }
            }

            foreach (FileInfo file in files.DistinctBy(x => x.Name))
            {
                PluginLoader loader = PluginLoader.CreateFromAssemblyFile(
                    file.FullName,
                    isUnloadable: false,
                    sharedTypes: [typeof(ICommandPlugin)],
                    config =>
                    {
                        config.IsLazyLoaded = false;
                        config.EnableHotReload = false;
                    });

                loaders.Add(loader);
            }

            List<ICommandPlugin> plugins = [];
            List<Exception> exceptions = [];
            bool containPlugin = false;

            foreach (PluginLoader loader in loaders)
            {
                try
                {
                    foreach (Type pluginType in loader
                                 .LoadDefaultAssembly()
                                 .GetTypes()
                                 .Where(t => typeof(ICommandPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        // This assumes the implementation of IPlugin has a parameterless constructor
                        if (Activator.CreateInstance(pluginType) is ICommandPlugin plugin)
                        {
                            containPlugin = true;
                            plugins.Add(plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                finally
                {
                    if (!containPlugin)
                    {
                        loader.Dispose();
                    }

                    containPlugin = false;
                }
            }

            loaders.Clear();
            files.Clear();

            if (this.RaiseExceptions && exceptions.Count > 0)
            {
                throw new AggregateException("One or more plugins failed to load", exceptions);
            }

            return plugins;
        }
    }
}