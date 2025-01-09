// <copyright file="UninstallTemplateCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Threading.Tasks;

using Spectre.Console;
using Spectre.Console.Cli;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Abstractions.Templates;
using Vellum.Cli.Templates;

namespace Vellum.Cli.Commands.Templates;

public class UninstallTemplateCommand(IAppEnvironment appEnvironment) : AsyncCommand<UninstallTemplateCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.WriteLine($"Uninstalling template package '{settings.PackageId}'");

        TemplateSettingsManager templateSettingsManager = new(appEnvironment);
        TemplatesSettings currentSettings = templateSettingsManager.LoadSettings(nameof(TemplatesSettings)) ?? new TemplatesSettings();

        var packageManager = new NuGetTemplatePackageManager(appEnvironment);

        if (currentSettings.Packages.Exists(templatePackage => templatePackage.PackageId == settings.PackageId))
        {
            TemplatePackage? package = currentSettings.Packages.Find(templatePackage => templatePackage.Id == settings.PackageId);

            if (package != null)
            {
                await packageManager.UnInstallAsync(package).ConfigureAwait(false);

                currentSettings.Packages.Remove(package);

                if (currentSettings.DefaultTemplate.PackageName == package.PackageId)
                {
                    currentSettings.DefaultTemplate = new DefaultTemplate();
                }
            }
        }

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<PACKAGE_ID>")]
        [Description("The ID of the package to uninstall.")]
        public string? PackageId { get; set; }
    }
}