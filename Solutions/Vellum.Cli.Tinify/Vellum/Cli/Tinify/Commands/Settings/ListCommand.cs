// <copyright file="ListCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Tinify.Environment;
using Vellum.Cli.Tinify.Settings;

namespace Vellum.Cli.Tinify.Commands.Settings;

public class ListCommand : Command
{
    public override int Execute(CommandContext context)
    {
        TinifySettingsManager settingsManager = new(new FileSystemRoamingProfileAppEnvironment());
        TinifySettings settings = settingsManager.LoadSettings(nameof(TinifySettings));

        if (settings != null)
        {
            AnsiConsole.WriteLine($"Tinify Key: {settings.Key}");

            return ReturnCodes.Ok;
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Tinify Value cannot be found. Please Run:[/]");
            AnsiConsole.MarkupLine("[yellow]vellum-cli tinify setting update <VALUE>[/]");

            return ReturnCodes.Error;
        }
    }
}