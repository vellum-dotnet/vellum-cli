// <copyright file="ListCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Cloudinary.Settings
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Cloudinary.Environment;
    using Vellum.Cli.Cloudinary.Upload;

    public static class ListCommandHandler
    {
        public static Task<int> Execute(IConsole console, InvocationContext context = null)
        {
            var settingsManager = new CloudinarySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            CloudinarySettings settings = settingsManager.LoadSettings();

            if (settings != null)
            {
                console.Out.WriteLine($"Cloudinary Settings:");
                console.Out.WriteLine($"cloud: {settings.Cloud}");
                console.Out.WriteLine($"key: {settings.Key}");
                console.Out.WriteLine($"secret: {settings.Secret}");
            }
            else
            {
                console.Out.WriteLine($"Cloudinary Settings cannot be found. Please Run:");
                console.Out.WriteLine($"vellum cloudinary setting update --cloud <VALUE> --key <VALUE> --secret <VALUE>");
            }

            return Task.FromResult(0);
        }
    }
}