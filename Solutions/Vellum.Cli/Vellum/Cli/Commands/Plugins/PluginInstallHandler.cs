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
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Installing plugin from package {options.PackageId}");

            var packageManager = new NuGetPluginPackageManager(appEnvironment);

            PluginPackageMetaData result = await packageManager.InstallLatestAsync(options.PackageId).ConfigureAwait(false);

            console.Out.WriteLine($"Installed plugin {result.Name} to {result.PluginPath}");

            return ReturnCodes.Ok;
        }
    }
}
