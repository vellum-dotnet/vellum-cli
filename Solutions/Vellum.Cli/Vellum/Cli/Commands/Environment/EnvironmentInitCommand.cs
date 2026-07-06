// <copyright file="EnvironmentInitCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Commands.Environment;

public class EnvironmentInitCommand(IAppEnvironment appEnvironment) : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await appEnvironment.InitializeAsync().ConfigureAwait(false);

        return ReturnCodes.Ok;
    }
}