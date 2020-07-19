// <copyright file="OptimizeCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Tinify.Commands.Optimize
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.IO;
    using System.Threading.Tasks;
    using TinifyAPI;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Tinify.Environment;
    using Vellum.Cli.Tinify.Settings;

    public static class OptimizeCommandHandler
    {
        public static async Task<int> Execute(OptimizeOptions options, IConsole console, InvocationContext context = null)
        {
            if (options == null)
            {
                console.Error.WriteLine("Invalid File Path");
                return ReturnCodes.Error;
            }

            var settingsManager = new TinifySettingsManager(new FileSystemRoamingProfileAppEnvironment());

            TinifySettings settings = settingsManager.LoadSettings();

            try
            {
                console.Out.WriteLine("Validating Tinify API Key");

                Tinify.Key = settings.Key;
                bool validKey = await Tinify.Validate().ConfigureAwait(false);

                if (!validKey)
                {
                    console.Error.WriteLine("Invalid Tinify API Key");
                    return ReturnCodes.Error;
                }

                long originalSizeInBytes = options.FilePath.Length;

                console.Out.WriteLine($"Original size: {originalSizeInBytes / 1024}KB");

                Source source = await Tinify.FromFile(options.FilePath.FullName).ConfigureAwait(false);
                await source.ToFile(options.FilePath.FullName);

                long newSizeInBytes =  new FileInfo(options.FilePath.FullName).Length;
                double percentChange = (newSizeInBytes - originalSizeInBytes) * 100.0 / originalSizeInBytes;

                console.Out.WriteLine($"New size: {newSizeInBytes / 1024}KB");
                console.Out.WriteLine($"-{percentChange}% Reduction");
            }
            catch (Exception exception)
            {
                console.Error.WriteLine(exception.Message);

                return ReturnCodes.Exception;
            }

            return ReturnCodes.Ok;
        }
    }
}