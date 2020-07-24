// <copyright file="UpdateCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
        public static Task<int> Execute(UpdateOptions options, IConsole console, InvocationContext context = null)
        {
            var settingsManager = new TinifySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            var settings = new TinifySettings
            {
                Key = options.Key,
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

            return Task.FromResult(0);
        }
    }
}