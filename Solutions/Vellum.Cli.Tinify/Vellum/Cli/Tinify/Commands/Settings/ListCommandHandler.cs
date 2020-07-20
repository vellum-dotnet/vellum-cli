// <copyright file="ListCommandHandler.cs" company="Endjin Limited">
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

    public static class ListCommandHandler
    {
        public static Task<int> Execute(IConsole console, InvocationContext context = null)
        {
            var settingsManager = new TinifySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            TinifySettings settings = settingsManager.LoadSettings();

            if (settings != null)
            {
                console.Out.WriteLine($"Tinify Key: {settings.Key}");
            }
            else
            {
                console.Out.WriteLine($"Tinify Settings cannot be found. Please Run:");
                console.Out.WriteLine($"vellum-cli tinify setting update <VALUE>");
            }

            return Task.FromResult(0);
        }
    }
}