// <copyright file="PluginInstallHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Packages;

    public static class PluginInstallHandler
    {
        public static async Task<int> ExecuteAsync(
            PluginOptions options,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var packageManager = new NuGetPluginPackageManager(appEnvironment);
            var result = await packageManager.InstallLatestAsync(options.PackageId).ConfigureAwait(false);

            return ReturnCodes.Ok;
        }
    }
}
