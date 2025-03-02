﻿// <copyright file="UpdateCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Tinify.Environment;
using Vellum.Cli.Tinify.Settings;

namespace Vellum.Cli.Tinify.Commands.Settings;

public class UpdateCommand : Command<UpdateCommand.Settings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        TinifySettingsManager settingsManager = new(new FileSystemRoamingProfileAppEnvironment());
        TinifySettings tinifySettings = new(settings.Key);

        try
        {
            settingsManager.SaveSettings(tinifySettings, nameof(TinifySettings));
            AnsiConsole.WriteLine("Settings updated.");

            return ReturnCodes.Ok;
        }
        catch
        {
            AnsiConsole.WriteLine("Settings could not be updated.");

            return ReturnCodes.Error;
        }
    }

    public class Settings : CommandSettings
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        [CommandOption("--key")]
        [Description("Tinify API Key")]
        public string? Key { get; init; }
    }
}