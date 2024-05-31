// <copyright file="PluginUninstallCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Packages;

namespace Vellum.Cli.Commands.Plugins;

public class PluginUninstallCommand(IAppEnvironment appEnvironment)
    : AsyncCommand<PluginUninstallCommand.PluginUninstallSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, PluginUninstallSettings settings)
    {
        AnsiConsole.WriteLine($"Uninstalling plugin with package id '{settings.PackageId}'");

        var packageManager = new NuGetPluginPackageManager(appEnvironment);

        try
        {
            if (settings.PackageId is null)
            {
                AnsiConsole.WriteLine("No package ID specified.");
                return ReturnCodes.Error;
            }

            await packageManager.UninstallAsync(settings.PackageId).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            AnsiConsole.WriteLine(exception.Message);
            return ReturnCodes.Exception;
        }

        return ReturnCodes.Ok;
    }

    public class PluginUninstallSettings : CommandSettings
    {
        [CommandArgument(0, "<PACKAGE_ID>")]
        [Description("The package id of the plugin to uninstall.")]
        public string? PackageId { get; set; }
    }
}