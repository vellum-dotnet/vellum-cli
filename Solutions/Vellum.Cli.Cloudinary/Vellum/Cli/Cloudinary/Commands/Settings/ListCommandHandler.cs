// <copyright file="ListCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Cloudinary.Commands.Settings
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Cloudinary.Environment;
    using Vellum.Cli.Cloudinary.Settings;

    public static class ListCommandHandler
    {
        public static Task<int> Execute(IConsole console, InvocationContext context = null)
        {
            var settingsManager = new CloudinarySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            CloudinarySettings settings = settingsManager.LoadSettings();

            if (settings != null)
            {
                console.Out.WriteLine("Cloudinary Value:");
                console.Out.WriteLine($"cloud: {settings.Cloud}");
                console.Out.WriteLine($"key: {settings.Key}");
                console.Out.WriteLine($"secret: {settings.Secret}");
            }
            else
            {
                console.Out.WriteLine("Cloudinary Value cannot be found. Please Run:");
                console.Out.WriteLine("vellum cloudinary setting update --cloud <VALUE> --key <VALUE> --secret <VALUE>");
            }

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}