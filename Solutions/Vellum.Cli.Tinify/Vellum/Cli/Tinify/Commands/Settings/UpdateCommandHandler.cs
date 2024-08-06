// <copyright file="UpdateCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Tinify.Commands.Settings
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Tinify.Environment;
    using Vellum.Cli.Tinify.Settings;

    public static class UpdateCommandHandler
    {
        public static Task<int> Execute(string key, IConsole console, InvocationContext context = null)
        {
            var settingsManager = new TinifySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            var settings = new TinifySettings
            {
                Key = key,
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

            return Task.FromResult(0);
        }
    }
}