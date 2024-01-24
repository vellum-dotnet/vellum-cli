// <copyright file="UpdateCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel;

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Cloudinary.Environment;
using Vellum.Cli.Cloudinary.Settings;

namespace Vellum.Cli.Cloudinary.Commands.Settings;

public class UpdateCommand : Command<UpdateCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        CloudinarySettingsManager settingsManager = new(new FileSystemRoamingProfileAppEnvironment());
        CloudinarySettings cloudinarySettings = new(settings.Cloud, settings.Key, settings.Secret);

        try
        {
            settingsManager.SaveSettings(cloudinarySettings, nameof(CloudinarySettings));
            AnsiConsole.WriteLine("Settings updated.");
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]Settings could not be updated.[/]");
        }

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        [CommandOption("--cloud|-c")]
        [Description("Cloudinary Cloud Account PackageId")]
        public string? Cloud { get; set; }

        [CommandOption("--key|-k")]
        [Description("Cloudinary API Key")]
        public string? Key { get; set; }

        [CommandOption("--secret|-s")]
        [Description("Cloudinary API Secret")]
        public string? Secret { get; set; }
    }
}