// <copyright file="PluginClearHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins;

using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using NDepend.Path;
using Spectre.Console;
using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Abstractions.Infrastructure;
using Vellum.Cli.Packages;

public static class PluginClearHandler
{
    public static System.Threading.Tasks.Task<int> ExecuteAsync(
        ICompositeConsole console,
        IAppEnvironment appEnvironment,
        InvocationContext context = null)
    {
        var packageManager = new NuGetPluginPackageManager(appEnvironment);

        IEnumerable<IAbsoluteDirectoryPath> paths = appEnvironment.PluginPaths;

        foreach (IAbsoluteDirectoryPath directoryPath in paths)
        {
            console.MarkupLineInterpolated($"❌ DELETED: [red]{directoryPath.DirectoryInfo?.Parent?.FullName}[/]");
            directoryPath.DirectoryInfo?.Parent?.Delete(recursive: true);
        }

        return Task.FromResult(Vellum.Cli.Abstractions.ReturnCodes.Ok);
    }
}