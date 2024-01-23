// <copyright file="ListCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Cloudinary.Environment;
using Vellum.Cli.Cloudinary.Settings;

namespace Vellum.Cli.Cloudinary.Commands.Settings;

public class ListCommand : Command
{
    public override int Execute(CommandContext context)
    {
        CloudinarySettingsManager settingsManager = new(new FileSystemRoamingProfileAppEnvironment());

        CloudinarySettings settings = settingsManager.LoadSettings("TODO");

        if (settings != null)
        {
            AnsiConsole.WriteLine("Cloudinary Value:");
            AnsiConsole.WriteLine($"cloud: {settings.Cloud}");
            AnsiConsole.WriteLine($"key: {settings.Key}");
            AnsiConsole.WriteLine($"secret: {settings.Secret}");
        }
        else
        {
            AnsiConsole.WriteLine("Cloudinary Value cannot be found. Please Run:");
            AnsiConsole.WriteLine("vellum cloudinary setting update --cloud <VALUE> --key <VALUE> --secret <VALUE>");
        }

        return ReturnCodes.Ok;
    }
}