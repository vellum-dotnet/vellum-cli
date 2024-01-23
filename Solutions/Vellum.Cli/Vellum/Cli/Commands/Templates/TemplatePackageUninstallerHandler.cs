// <copyright file="TemplatePackageUninstallerHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
/*
#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Commands.Templates
{
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Templates;
    using Vellum.Cli.Templates;

    public static class TemplatePackageUninstallerHandler
    {
        public static async Task<int> ExecuteAsync(
            string packageId,
            Vellum.Cli.Abstractions.Infrastructure.ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            console.Out.WriteLine($"Uninstalling template package '{packageId}'");

            var templateSettingsManager = new TemplateSettingsManager(appEnvironment);
            TemplatesSettings currentSettings = templateSettingsManager.LoadSettings() ?? new TemplatesSettings();

            var packageManager = new NuGetTemplatePackageManager(appEnvironment);

            if (currentSettings.Packages.Exists(templatePackage => templatePackage.PackageId == packageId))
            {
                TemplatePackage package = currentSettings.Packages.Find(templatePackage => templatePackage.Id == packageId);

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
    }
}*/