// <copyright file="ListCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading;

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Cloudinary.Environment;
using Vellum.Cli.Cloudinary.Settings;

namespace Vellum.Cli.Cloudinary.Commands.Settings;

public class ListCommand : Command
{
    protected override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        CloudinarySettingsManager settingsManager = new(new FileSystemRoamingProfileAppEnvironment());

        CloudinarySettings settings = settingsManager.LoadSettings(nameof(CloudinarySettings));

        if (settings != null)
        {
            AnsiConsole.WriteLine("Cloudinary Value:");
            AnsiConsole.WriteLine($"cloud: {settings.Cloud}");
            AnsiConsole.WriteLine($"key: {settings.Key}");
            AnsiConsole.WriteLine($"secret: {settings.Secret}");

            return ReturnCodes.Ok;
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Cloudinary Value cannot be found. Please Run:[/]");
            AnsiConsole.MarkupLine("[yellow]vellum-cli cloudinary setting update --cloud <VALUE> --key <VALUE> --secret <VALUE>[/]");

            return ReturnCodes.Error;
        }
    }
}