// <copyright file="SetUsernameHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Environment;

    public static class SetUsernameHandler
    {
        public static Task<int> ExecuteAsync(
            UsernameOptions options,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var settingsManager = new EnvironmentSettingsManager(appEnvironment);

            EnvironmentSettings settings = settingsManager.LoadSettings() ?? new EnvironmentSettings();

            settings.Username = options.Username;

            settingsManager.SaveSettings(settings);

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}
