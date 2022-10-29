// <copyright file="TemplatePackageInstallerHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Templates
{
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
            string packageId,
            Vellum.Cli.Abstractions.Infrastructure.ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Installing template from package '{packageId}'");

            var templateSettingsManager = new TemplateSettingsManager(appEnvironment);
            TemplatesSettings currentSettings = templateSettingsManager.LoadSettings() ?? new TemplatesSettings();

            var packageManager = new NuGetTemplatePackageManager(appEnvironment);

            if (currentSettings.Packages.Exists(x => x.PackageId == packageId))
            {
                TemplatePackage package = currentSettings.Packages.Find(x => x.Id == packageId);

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

            TemplatePackage templatePackage = await packageManager.InstallLatestAsync(packageId).ConfigureAwait(false);

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