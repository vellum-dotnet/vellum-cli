// <copyright file="PluginListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Commands.Plugins
{
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using NDepend.Path;
    using Spectre.Console;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Infrastructure;

    public static class PluginListHandler
    {
        public static Task<int> ExecuteAsync(
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            foreach (IAbsoluteDirectoryPath pluginPath in appEnvironment.PluginPaths)
            {
                console.MarkupLineInterpolated($"-🔌{pluginPath.ParentDirectoryPath.DirectoryName} ([green]{pluginPath.DirectoryName}[/])");
            }

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}