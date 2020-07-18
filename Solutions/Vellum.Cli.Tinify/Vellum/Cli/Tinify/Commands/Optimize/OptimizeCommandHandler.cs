// <copyright file="OptimizeCommandHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Tinify.Commands.Optimize
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
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
                Tinify.Key = settings.Key;
                bool validKey = await Tinify.Validate().ConfigureAwait(false);

                if (!validKey)
                {
                    console.Error.WriteLine("Invalid Tinify API Key");
                    return ReturnCodes.Error;
                }

                long originalSizeInBytes = options.FilePath.Length;

                Source source = await Tinify.FromFile(options.FilePath.FullName).ConfigureAwait(false);
                await source.ToFile(options.FilePath.FullName);

                long newSizeInBytes = options.FilePath.Length;
                double percentChange = (newSizeInBytes - originalSizeInBytes) * 100.0 / originalSizeInBytes;
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