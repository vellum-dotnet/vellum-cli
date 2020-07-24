// <copyright file="UpdateCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Cloudinary.Commands.Settings
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Cloudinary.Environment;
    using Vellum.Cli.Cloudinary.Settings;

    public static class UpdateCommandHandler
    {
        public static Task<int> Execute(UpdateOptions options, IConsole console, InvocationContext context = null)
        {
            var settingsManager = new CloudinarySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            var settings = new CloudinarySettings
            {
                Cloud = options.Cloud,
                Key = options.Key,
                Secret = options.Secret,
            };
            try
            {
                settingsManager.SaveSettings(settings);
                console.Out.WriteLine($"Settings updated.");
            }
            catch
            {
                console.Error.WriteLine($"Settings could not be updated.");
            }

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}