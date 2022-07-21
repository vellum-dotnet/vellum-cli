// <copyright file="PluginInstallHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Plugins;
    using Vellum.Cli.Packages;

    public static class PluginInstallHandler
    {
        public static async Task<int> ExecuteAsync(
            string packageId,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Installing plugin from package '{packageId}'");

            var packageManager = new NuGetPluginPackageManager(appEnvironment);

            try
            {
                PluginPackage result = await packageManager.InstallLatestAsync(packageId).ConfigureAwait(false);

                console.Out.WriteLine($"Using plugin version {result.Version}");
                console.Out.WriteLine($"Installed plugin {result.Name} to {result.PluginPath}");
            }
            catch (System.IO.IOException)
            {
                console.Out.WriteLine("The latest version of this plugin is already installed.");
                return ReturnCodes.Error;
            }

            return ReturnCodes.Ok;
        }
    }
}
