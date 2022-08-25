// <copyright file="SetEnvironmentSettingHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Infrastructure;

    public static class SetEnvironmentSettingHandler
    {
        public static Task<int> ExecuteAsync(
            string username,
            DirectoryInfo workspacePath,
            DirectoryInfo publishPath,
            string key,
            string value,
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var settingsManager = new EnvironmentSettingsManager(appEnvironment);

            EnvironmentSettings settings = settingsManager.LoadSettings() ?? new EnvironmentSettings();

            if (username != null)
            {
                settings.Username = username.ToLowerInvariant();
            }

            if (workspacePath != null)
            {
                settings.WorkspacePath = workspacePath.FullName;
            }

            if (publishPath != null)
            {
                settings.PublishPath = publishPath.FullName;
            }

            if (key != null && value != null)
            {
                if (settings.Configuration.ContainsKey(key))
                {
                    settings.Configuration[key] = value;
                }
                else
                {
                    settings.Configuration.Add(key, value);
                }
            }

            settingsManager.SaveSettings(settings);

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}