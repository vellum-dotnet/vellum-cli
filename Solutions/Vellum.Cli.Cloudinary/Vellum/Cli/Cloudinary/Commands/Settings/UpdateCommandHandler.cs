// <copyright file="UpdateCommandHandler.cs" company="Endjin Limited">
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

    public static class UpdateCommandHandler
    {
        public static Task<int> Execute(string cloud, string key, string secret, IConsole console, InvocationContext context = null)
        {
            var settingsManager = new CloudinarySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            var settings = new CloudinarySettings
            {
                Cloud = cloud,
                Key = key,
                Secret = secret,
            };
            try
            {
                settingsManager.SaveSettings(settings);
                console.Out.WriteLine("Settings updated.");
            }
            catch
            {
                console.Error.WriteLine("Settings could not be updated.");
            }

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}