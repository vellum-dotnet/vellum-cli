// <copyright file="PluginUninstallHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
/*
#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Commands.Plugins
{
    using System;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Packages;

    public static class PluginUninstallHandler
    {
        public static async Task<int> ExecuteAsync(
            string packageId,
            Vellum.Cli.Abstractions.Infrastructure.ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Uninstalling plugin with package id '{packageId}'");

            var packageManager = new NuGetPluginPackageManager(appEnvironment);

            try
            {
                await packageManager.UninstallAsync(packageId).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                console.Error.WriteLine(exception.Message);
                return ReturnCodes.Exception;
            }

            return ReturnCodes.Ok;
        }
    }
}*/