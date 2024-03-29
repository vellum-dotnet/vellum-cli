﻿// <copyright file="SetEnvironmentSettingHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

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
                settings.Configuration[key] = value;
            }

            settingsManager.SaveSettings(settings);

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}