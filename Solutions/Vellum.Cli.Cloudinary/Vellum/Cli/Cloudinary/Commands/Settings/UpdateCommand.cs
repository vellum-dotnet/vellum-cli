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

        CloudinarySettings cloudinarySettings = new()
        {
            Cloud = settings.Cloud,
            Key = settings.Key,
            Secret = settings.Secret,
        };
        try
        {
            settingsManager.SaveSettings(cloudinarySettings, "TODO");
            AnsiConsole.WriteLine("Settings updated.");
        }
        catch
        {
            AnsiConsole.WriteLine("Settings could not be updated.");
        }

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<CLOUD>")]
        [Description("Cloudinary Cloud Account PackageId")]
        public string Cloud { get; set; } = null!;

        [CommandArgument(1, "<KEY>")]
        [Description("Cloudinary API Key")]
        public string Key { get; set; } = null!;

        [CommandArgument(2, "<SECRET>")]
        [Description("Cloudinary API Secret")]
        public string Secret { get; set; } = null!;
    }
}