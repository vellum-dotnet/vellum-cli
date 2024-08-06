// <copyright file="EnvironmentInitHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Infrastructure;

    public static class EnvironmentInitHandler
    {
        public static async Task<int> ExecuteAsync(ICompositeConsole console, IAppEnvironment appEnvironment)
        {
            await appEnvironment.InitializeAsync(console).ConfigureAwait(false);

            return ReturnCodes.Ok;
        }
    }
}