// <copyright file="EnvironmentSettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Environment
{
    using Vellum.Cli.Abstractions.Configuration;

    public class EnvironmentSettingsManager : SettingsManager<EnvironmentSettings>
    {
        public EnvironmentSettingsManager(IAppEnvironmentConfiguration appEnvironment)
            : base(appEnvironment)
        {
        }
    }
}