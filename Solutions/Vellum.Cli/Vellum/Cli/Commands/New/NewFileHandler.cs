// <copyright file="NewHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.New
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Conventions;
    using Vellum.Cli.Abstractions.Environment;

    public static class NewHandler
    {
        public static async Task<int> ExecuteAsync(
            NewOptions options,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var conventionsManager = new ConventionsManager();

            foreach (var pluginPath in appEnvironment.PluginPaths)
            {
                var conventions = await conventionsManager.LoadAsync(pluginPath.GetChildFileWithName("conventions.json")).ConfigureAwait(false);
            }

            return ReturnCodes.Ok;
        }
    }
}
