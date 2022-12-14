// <copyright file="UploadCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Cloudinary.Commands.Upload
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.IO;
    using System.Threading.Tasks;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Cloudinary.Environment;
    using Vellum.Cli.Cloudinary.Settings;

    public static class UploadCommandHandler
    {
        public static async Task<int> Execute(FileInfo file, IConsole console, InvocationContext context = null)
        {
            if (file == null)
            {
                console.Error.WriteLine("Invalid File TemplateRepositoryPath");
                return ReturnCodes.Error;
            }

            var settingsManager = new CloudinarySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            CloudinarySettings settings = settingsManager.LoadSettings();

            var cloudinary = new Cloudinary(new Account(settings.Cloud, settings.Key, settings.Secret));

            var fileToUpload = new ImageUploadParams
            {
                File = new FileDescription(file.FullName),
                PublicId = $"assets/images/blog/{DateTime.Now.Year}/{DateTime.Now.Month:00}/{Path.GetFileNameWithoutExtension(file.Name.ToLowerInvariant())}",
                UniqueFilename = false,
                UseFilename = false,
            };

            try
            {
                ImageUploadResult uploadResult = await cloudinary.UploadAsync(fileToUpload).ConfigureAwait(false);

                console.Out.WriteLine("Image uploaded.");
                console.Out.WriteLine($"Use following path in your blog post: /{uploadResult.PublicId}{Path.GetExtension(file.Name)}");
            }
            catch (Exception exception)
            {
                console.Error.WriteLine(exception.Message);

                return ReturnCodes.Error;
            }

            return ReturnCodes.Ok;
        }
    }
}