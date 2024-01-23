// <copyright file="CloudinarySettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Vellum.Cli.Abstractions.Configuration;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Cloudinary.Settings;

public class CloudinarySettingsManager(IAppEnvironmentConfiguration appEnvironment)
    : SettingsManager<CloudinarySettings>(appEnvironment);