// <copyright file="SetEnvironmentSettingCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.IO;

using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Commands.Environment;

public class SetEnvironmentSettingCommand(IAppEnvironmentConfiguration appEnvironmentConfiguration) : Command<SetEnvironmentSettingCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        var settingsManager = new EnvironmentSettingsManager(appEnvironmentConfiguration);

        EnvironmentSettings envSettings = settingsManager.LoadSettings(nameof(EnvironmentSettings)) ?? new EnvironmentSettings();

        if (settings.Username != null)
        {
            envSettings.Username = settings.Username.ToLowerInvariant();
        }

        if (settings.WorkspacePath != null)
        {
            envSettings.WorkspacePath = settings.WorkspacePath.FullName;
        }

        if (settings.PublishPath != null)
        {
            envSettings.PublishPath = settings.PublishPath.FullName;
        }

        if (settings is { Key: not null, Value: not null })
        {
            envSettings.Configuration[settings.Key] = settings.Value;
        }

        settingsManager.SaveSettings(envSettings, nameof(EnvironmentSettings));

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        [CommandOption("--username|-u")]
        [Description("Username")]
        public string? Username { get; set; }

        [CommandOption("--workspace|-w")]
        [Description("Workspace Path")]
        public DirectoryInfo? WorkspacePath { get; set; }

        [CommandOption("--publish|-p")]
        [Description("Publish Path")]
        public DirectoryInfo? PublishPath { get; set; }

        [CommandOption("--key|-k")]
        [Description("Configuration Key")]
        public string? Key { get; set; }

        [CommandOption("--value|-v")]
        [Description("Configuration Value")]
        public string? Value { get; set; }
    }
}