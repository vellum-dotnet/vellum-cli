// <copyright file="PluginUninstallHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Packages;

    public static class PluginUninstallHandler
    {
        public static async Task<int> ExecuteAsync(
            PluginOptions options,
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Uninstalling plugin with package id '{options.PackageId}'");

            var packageManager = new NuGetPluginPackageManager(appEnvironment);

            try
            {
                await packageManager.UninstallAsync(options.PackageId).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                console.Error.WriteLine(exception.Message);
                return ReturnCodes.Exception;
            }

            return ReturnCodes.Ok;
        }
    }
}