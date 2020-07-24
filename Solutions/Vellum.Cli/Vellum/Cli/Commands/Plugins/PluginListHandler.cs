// <copyright file="PluginListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Templates;
    using Vellum.Cli.Packages;

    public static class PluginListHandler
    {
        public static Task<int> ExecuteAsync(
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            /*var packageManager = new NuGetPluginPackageManager(appEnvironment);
            TemplatePackage result = await packageManager.InstallLatestAsync(options.PackageId).ConfigureAwait(false);*/

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}