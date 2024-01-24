// <copyright file="PluginInstallCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Abstractions.Plugins;
using Vellum.Cli.Packages;

namespace Vellum.Cli.Commands.Plugins;

public class PluginInstallCommand(IAppEnvironment appEnvironment) : AsyncCommand<PluginInstallCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        string message = $"Installing plugin from package '{settings.PackageId}'";

        if (!string.IsNullOrEmpty(settings.Version))
        {
            message += " version " + settings.Version;
        }

        AnsiConsole.WriteLine(message);

        var packageManager = new NuGetPluginPackageManager(appEnvironment);

        try
        {
            PluginPackage? result = null;

            if (settings.PackageId is null)
            {
                AnsiConsole.WriteLine("No package ID specified.");
                return ReturnCodes.Error;
            }

            AnsiConsole.WriteLine($"Using plugin version {result?.Version}");

            if (string.IsNullOrEmpty(settings.Version))
            {
                result = await packageManager.InstallLatestAsync(settings.PackageId).ConfigureAwait(false);
            }
            else
            {
                result = await packageManager.InstallVersionAsync(settings.PackageId, settings.Version).ConfigureAwait(false);
            }

            AnsiConsole.WriteLine($"Installed plugin {result?.Name} to {result?.PluginPath}");
        }
        catch (System.IO.IOException)
        {
            AnsiConsole.WriteLine("The latest version of this plugin is already installed.");
            return ReturnCodes.Error;
        }
        catch (InvalidOperationException exception)
        {
            AnsiConsole.MarkupLineInterpolated($"⚠️ [red]ERROR! {exception.Message}[/]");
            return ReturnCodes.Error;
        }

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<PACKAGE_ID>")]
        [Description("The package ID of the plugin to install.")]
        public string? PackageId { get; set; }

        [CommandArgument(1, "[VERSION]")]
        [Description("The version of the plugin to install.")]
        public string? Version { get; set; }
    }
}