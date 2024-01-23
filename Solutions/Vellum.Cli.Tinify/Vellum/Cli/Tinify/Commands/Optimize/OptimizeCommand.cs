// <copyright file="OptimizeCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Tinify.Environment;
using Vellum.Cli.Tinify.Settings;

namespace Vellum.Cli.Tinify.Commands.Optimize;

public class OptimizeCommand : AsyncCommand<OptimizeCommand.Settings>
{
    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (settings.FilePath == null)
        {
            AnsiConsole.WriteLine("Invalid File TemplateRepositoryPath");
            return ReturnCodes.Error;
        }

        var settingsManager = new TinifySettingsManager(new FileSystemRoamingProfileAppEnvironment());

        TinifySettings tinifySettings = settingsManager.LoadSettings(nameof(TinifySettings));

        try
        {
            AnsiConsole.WriteLine("Validating Tinify API Key");

            TinifyAPI.Tinify.Key = tinifySettings.Key;
            bool validKey = await TinifyAPI.Tinify.Validate().ConfigureAwait(false);

            if (!validKey)
            {
                AnsiConsole.WriteLine("Invalid Tinify API Key");
                return ReturnCodes.Error;
            }

            FileInfo fileInfo = new(settings.FilePath.FullPath);

            long originalSizeInBytes = fileInfo.Length;

            AnsiConsole.WriteLine($"Original size: {originalSizeInBytes / 1024}KB");

            TinifyAPI.Source source = await TinifyAPI.Tinify.FromFile(fileInfo.FullName).ConfigureAwait(false);
            await source.ToFile(fileInfo.FullName);

            long newSizeInBytes = new FileInfo(fileInfo.FullName).Length;
            double percentChange = (newSizeInBytes - originalSizeInBytes) * 100.0 / originalSizeInBytes;

            AnsiConsole.WriteLine($"New size: {newSizeInBytes / 1024}KB");
            AnsiConsole.WriteLine($"{percentChange:00}% Reduction");
        }
        catch (Exception exception)
        {
            AnsiConsole.WriteLine(exception.Message);

            return ReturnCodes.Exception;
        }

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        [CommandOption("--file-path")]
        [Description("Which image file (jpg|png) are you going to optimize?")]
        public FilePath FilePath { get; init; }
    }
}