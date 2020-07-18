// <copyright file="AppEnvironmentManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Environment
{
    using System.CommandLine;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions.Templates;

    public class AppEnvironmentManager : IAppEnvironmentManager
    {
        private readonly IAppEnvironment appEnvironment;
        private readonly ITemplatePackageManager templateManager;
        private readonly ITemplateSettingsManager templateSettingsManager;

        public AppEnvironmentManager(IAppEnvironment appEnvironment, ITemplatePackageManager templateManager, ITemplateSettingsManager templateSettingsManager)
        {
            this.appEnvironment = appEnvironment;
            this.templateManager = templateManager;
            this.templateSettingsManager = templateSettingsManager;
        }

        public async Task SetDesiredStateAsync(IConsole console)
        {
            const string defaultPackageId = "adr.templates";

            await this.appEnvironment.InitializeAsync(console).ConfigureAwait(false);

            TemplatePackageMetaData templateMetaData = await this.templateManager.InstallLatestAsync(defaultPackageId).ConfigureAwait(false);

            var templateSettings = new TemplateSettings
            {
                MetaData = templateMetaData,
                DefaultTemplate = templateMetaData.Details.Find(x => x.IsDefault)?.FullPath,
                DefaultTemplatePackage = defaultPackageId,
            };

            this.templateSettingsManager.SaveSettings(templateSettings);
        }

        public async Task SetFirstRunDesiredStateAsync(IConsole console)
        {
            if (!this.appEnvironment.IsInitialized())
            {
                await this.SetDesiredStateAsync(console).ConfigureAwait(false);
            }
        }

        public async Task ResetDesiredStateAsync(IConsole console)
        {
            this.appEnvironment.Clean();
            await this.SetDesiredStateAsync(console).ConfigureAwait(false);
        }
    }
}