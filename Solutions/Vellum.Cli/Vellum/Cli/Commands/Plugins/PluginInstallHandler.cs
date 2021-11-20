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
            PluginOptions options,
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Installing plugin from package '{options.PackageId}'");

            var packageManager = new NuGetPluginPackageManager(appEnvironment);

            try
            {
                PluginPackage result = await packageManager.InstallLatestAsync(options.PackageId).ConfigureAwait(false);

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
