// <copyright file="SetEnvironmentSettingHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;

    public static class SetEnvironmentSettingHandler
    {
        public static Task<int> ExecuteAsync(
            SetOptions options,
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var settingsManager = new EnvironmentSettingsManager(appEnvironment);

            EnvironmentSettings settings = settingsManager.LoadSettings() ?? new EnvironmentSettings();

            if (options.Username != null)
            {
                settings.Username = options.Username.ToLowerInvariant();
            }

            if (options.WorkspacePath != null)
            {
                settings.WorkspacePath = options.WorkspacePath.FullName;
            }

            if (options.PublishPath != null)
            {
                settings.PublishPath = options.PublishPath.FullName;
            }

            if (options.Key != null && options.Value != null)
            {
                if (settings.Configuration.ContainsKey(options.Key))
                {
                    settings.Configuration[options.Key] = options.Value;
                }
                else
                {
                    settings.Configuration.Add(options.Key, options.Value);
                }
            }

            settingsManager.SaveSettings(settings);

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}
