// <copyright file="CloudinarySettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Cloudinary.Settings
{
    using Vellum.Cli.Abstractions.Configuration;
    using Vellum.Cli.Abstractions.Environment;

    public class CloudinarySettingsManager : SettingsManager<CloudinarySettings>
    {
        public CloudinarySettingsManager(IAppEnvironmentConfiguration appEnvironment)
            : base(appEnvironment)
        {
        }
    }
}