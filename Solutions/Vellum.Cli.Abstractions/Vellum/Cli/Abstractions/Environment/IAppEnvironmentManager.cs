// <copyright file="IAppEnvironmentManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Environment
{
    using System.CommandLine;
    using System.Threading.Tasks;

    public interface IAppEnvironmentManager
    {
        Task ResetDesiredStateAsync(IConsole console);

        Task SetDesiredStateAsync(IConsole console);

        Task SetFirstRunDesiredStateAsync(IConsole console);
    }
}