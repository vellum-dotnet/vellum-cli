// <copyright file="TemplateSettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Templates
{
    using Vellum.Cli.Abstractions.Configuration;
    using Vellum.Cli.Abstractions.Environment;

    public class TemplateSettingsManager : SettingsManager<TemplateSettings>, ITemplateSettingsManager
    {
        public TemplateSettingsManager(IAppEnvironment appEnvironment)
            : base(appEnvironment)
        {
        }
    }
}