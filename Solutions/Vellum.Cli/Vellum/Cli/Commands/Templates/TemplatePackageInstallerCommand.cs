// <copyright file="TemplatePackageInstallerCommand.cs" company="Endjin Limited">
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

public class TemplatePackageInstallerCommand(IAppEnvironment appEnvironment)
    : AsyncCommand<TemplatePackageInstallerCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.WriteLine($"Installing template from package '{settings.PackageId}'");

        TemplateSettingsManager templateSettingsManager = new(appEnvironment);
        TemplatesSettings currentSettings = templateSettingsManager.LoadSettings(nameof(TemplatesSettings)) ?? new TemplatesSettings();

        NuGetTemplatePackageManager packageManager = new(appEnvironment);

        if (currentSettings.Packages.Exists(x => x.PackageId == settings.PackageId))
        {
            TemplatePackage? package = currentSettings.Packages.Find(x => x.Id == settings.PackageId);

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

        if (settings.PackageId is null)
        {
            AnsiConsole.WriteLine("No package ID specified.");
            return ReturnCodes.Error;
        }

        TemplatePackage templatePackage = await packageManager.InstallLatestAsync(settings.PackageId).ConfigureAwait(false);

        if (!currentSettings.Packages.Exists(x => x.Id == templatePackage.Id))
        {
            currentSettings.Packages.Add(templatePackage);
        }

        currentSettings.DefaultTemplate ??= new DefaultTemplate
        {
            ContentType = templatePackage.Templates[0].ContentType,
            PackageName = templatePackage.PackageId,
            PackagePath = templatePackage.InstallationPath,
        };

        templateSettingsManager.SaveSettings(currentSettings, nameof(TemplatesSettings));

        return ReturnCodes.Ok;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<PACKAGE_ID>")]
        [Description("The package ID to install.")]
        public string? PackageId { get; set; }
    }
}