// <copyright file="TemplatePackageInstallerHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Templates
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Templates;
    using Vellum.Cli.Templates;

    public static class TemplatePackageInstallerHandler
    {
        public static async Task<int> ExecuteAsync(
            TemplateOptions options,
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Installing template from package '{options.PackageId}'");

            var templateSettingsManager = new TemplateSettingsManager(appEnvironment);
            TemplatesSettings currentSettings = templateSettingsManager.LoadSettings() ?? new TemplatesSettings();

            var packageManager = new NuGetTemplatePackageManager(appEnvironment);

            if (currentSettings.Packages.Exists(x => x.PackageId == options.PackageId))
            {
                TemplatePackage package = currentSettings.Packages.Find(x => x.Id == options.PackageId);

                if (package != null)
                {
                    await packageManager.UnnstallAsync(package).ConfigureAwait(false);

                    currentSettings.Packages.Remove(package);

                    if (currentSettings.DefaultTemplate.PackageName == package.PackageId)
                    {
                        currentSettings.DefaultTemplate = new DefaultTemplate();
                    }
                }
            }

            TemplatePackage templatePackage =
                await packageManager.InstallLatestAsync(options.PackageId).ConfigureAwait(false);

            if (!currentSettings.Packages.Exists(x => x.Id == templatePackage.Id))
            {
                currentSettings.Packages.Add(templatePackage);
            }

            currentSettings.DefaultTemplate ??= new DefaultTemplate
            {
                ContentType = templatePackage.Templates.First().ContentType,
                PackageName = templatePackage.PackageId,
                PackagePath = templatePackage.InstalltionPath,
            };

            templateSettingsManager.SaveSettings(currentSettings);

            return ReturnCodes.Ok;
        }
    }
}