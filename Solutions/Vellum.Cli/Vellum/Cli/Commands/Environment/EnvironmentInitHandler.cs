// <copyright file="EnvironmentInitHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.CommandLine;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;

    public static class EnvironmentInitHandler
    {
        public static async Task<int> ExecuteAsync(
            IConsole console,
            IAppEnvironment appEnvironment)
        {
            await appEnvironment.InitializeAsync(console).ConfigureAwait(false);

            return ReturnCodes.Ok;
        }
    }
}