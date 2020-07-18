// <copyright file="TinifySettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Tinify.Settings
{
    using Vellum.Cli.Abstractions.Configuration;
    using Vellum.Cli.Abstractions.Environment;

    public class TinifySettingsManager : SettingsManager<TinifySettings>
    {
        public TinifySettingsManager(IAppEnvironmentConfiguration appEnvironment)
            : base(appEnvironment)
        {
        }
    }
}