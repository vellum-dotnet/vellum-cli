// <copyright file="EnvironmentInitHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Commands.Environment;

public static class EnvironmentInitHandler
{
    public static async Task<int> ExecuteAsync(IAppEnvironment appEnvironment)
    {
        await appEnvironment.InitializeAsync().ConfigureAwait(false);

        return ReturnCodes.Ok;
    }
}