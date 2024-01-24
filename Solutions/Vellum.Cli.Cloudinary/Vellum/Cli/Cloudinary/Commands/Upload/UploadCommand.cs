// <copyright file="UploadCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Cloudinary.Environment;
using Vellum.Cli.Cloudinary.Settings;

namespace Vellum.Cli.Cloudinary.Commands.Upload;

public class UploadCommand : AsyncCommand<UploadCommand.UploadCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, UploadCommandSettings settings)
    {
        if (settings.File == null)
        {
            AnsiConsole.WriteLine("Invalid File TemplateRepositoryPath");
            return ReturnCodes.Error;
        }

        CloudinarySettingsManager settingsManager = new(new FileSystemRoamingProfileAppEnvironment());
        CloudinarySettings cloudinarySettings = settingsManager.LoadSettings(nameof(CloudinarySettings));

        CloudinaryDotNet.Cloudinary cloudinary = new(new Account(cloudinarySettings.Cloud, cloudinarySettings.Key, cloudinarySettings.Secret));

        string cdnPath = settings.CdnPath ?? $"assets/images/blog/{DateTime.Now.Year}/{DateTime.Now.Month:00}/{Path.GetFileNameWithoutExtension(settings.File.Name.ToLowerInvariant())}";

        ImageUploadParams fileToUpload = new()
        {
            File = new FileDescription(settings.File.FullName),
            PublicId = cdnPath,
            UniqueFilename = false,
            UseFilename = false,
        };

        try
        {
            ImageUploadResult uploadResult = await cloudinary.UploadAsync(fileToUpload).ConfigureAwait(false);

            AnsiConsole.WriteLine("Image uploaded.");
            AnsiConsole.WriteLine($"Here's the path of the asset you uploaded: /{uploadResult.PublicId}{Path.GetExtension(settings.File.Name)}");
        }
        catch (Exception exception)
        {
            AnsiConsole.WriteLine(exception.Message);

            return ReturnCodes.Error;
        }

        return ReturnCodes.Ok;
    }

    public class UploadCommandSettings : CommandSettings
    {
        [CommandOption("--file-path|-f")]
        [Description("Which file should be uploaded to Cloudinary?")]
        public FileInfo? File { get; set; }

        [CommandOption("--cdn-path|-c")]
        [Description("Destination path on the CDN, i.e. assets/images/blog/ or /assets/images/news")]
        public string? CdnPath { get; set; }
    }
}