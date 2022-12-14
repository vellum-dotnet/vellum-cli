// <copyright file="PluginInstallHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Commands.Plugins
{
    using System;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Spectre.Console;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Plugins;
    using Vellum.Cli.Packages;

    public static class PluginInstallHandler
    {
        public static async Task<int> ExecuteAsync(
            string packageId,
            string version,
            Vellum.Cli.Abstractions.Infrastructure.ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            string message = $"Installing plugin from package '{packageId}'";

            if (!string.IsNullOrEmpty(version))
            {
                message += " version " + version;
            }

            console.Out.WriteLine(message);

            var packageManager = new NuGetPluginPackageManager(appEnvironment);

            try
            {
                PluginPackage result = null;

                if (string.IsNullOrEmpty(version))
                {
                    result = await packageManager.InstallLatestAsync(packageId).ConfigureAwait(false);
                }
                else
                {
                    result = await packageManager.InstallVersionAsync(packageId, version).ConfigureAwait(false);
                }

                console.Out.WriteLine($"Using plugin version {result.Version}");
                console.Out.WriteLine($"Installed plugin {result.Name} to {result.PluginPath}");
            }
            catch (System.IO.IOException)
            {
                console.Out.WriteLine("The latest version of this plugin is already installed.");
                return ReturnCodes.Error;
            }
            catch (InvalidOperationException exception)
            {
                console.MarkupLineInterpolated($"⚠️ [red]ERROR! {exception.Message}[/]");
                return ReturnCodes.Error;
            }

            return ReturnCodes.Ok;
        }
    }
}