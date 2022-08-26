// <copyright file="PluginClearHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins;

using System.CommandLine.Invocation;
using System.Threading.Tasks;
using NDepend.Path;
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

        var paths = appEnvironment.PluginPaths;

        foreach (IAbsoluteDirectoryPath directoryPath in paths)
        {
            directoryPath.DirectoryInfo.Delete(recursive: true);
        }

        return Task.FromResult(Vellum.Cli.Abstractions.ReturnCodes.Ok);
    }
}