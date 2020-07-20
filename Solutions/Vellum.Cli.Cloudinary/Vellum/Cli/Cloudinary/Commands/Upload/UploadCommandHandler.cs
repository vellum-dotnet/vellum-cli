// <copyright file="UploadCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
        public static async Task<int> Execute(UploadOptions options, IConsole console, InvocationContext context = null)
        {
            if (options == null)
            {
                console.Error.WriteLine("Invalid File Path");
                return ReturnCodes.Error;
            }

            var settingsManager = new CloudinarySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            CloudinarySettings settings = settingsManager.LoadSettings();

            var cloudinary = new Cloudinary(new Account(settings.Cloud, settings.Key, settings.Secret));

            var fileToUpload = new ImageUploadParams
            {
                File = new FileDescription(options.FilePath.FullName),
                PublicId = $"assets/images/blog/{DateTime.Now.Year}/{DateTime.Now.Month:00}/{Path.GetFileNameWithoutExtension(options.FilePath.Name.ToLowerInvariant())}",
                UniqueFilename = false,
                UseFilename = false,
            };

            try
            {
                ImageUploadResult uploadResult = await cloudinary.UploadAsync(fileToUpload).ConfigureAwait(false);

                console.Out.WriteLine("Image uploaded.");
                console.Out.WriteLine($"Use following path in your blog post: /{uploadResult.PublicId}{Path.GetExtension(options.FilePath.Name)}");
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