// <copyright file="EnvironmentSettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Environment
{
    using Vellum.Cli.Abstractions.Configuration;
    using Vellum.Cli.Abstractions.Environment;

    public class EnvironmentSettingsManager : SettingsManager<EnvironmentSettings>
    {
        public EnvironmentSettingsManager(IAppEnvironmentConfiguration appEnvironment)
            : base(appEnvironment)
        {
        }
    }
}