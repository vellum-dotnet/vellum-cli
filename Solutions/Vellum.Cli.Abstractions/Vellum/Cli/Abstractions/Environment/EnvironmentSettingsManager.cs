// <copyright file="EnvironmentSettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Vellum.Cli.Abstractions.Configuration;

namespace Vellum.Cli.Abstractions.Environment;

public class EnvironmentSettingsManager : SettingsManager<EnvironmentSettings>
{
    public EnvironmentSettingsManager(IAppEnvironmentConfiguration appEnvironment)
        : base(appEnvironment)
    {
        }
}