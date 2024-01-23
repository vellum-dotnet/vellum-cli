// <copyright file="TinifySettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Vellum.Cli.Abstractions.Configuration;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Tinify.Settings;

public class TinifySettingsManager(IAppEnvironmentConfiguration appEnvironment)
    : SettingsManager<TinifySettings>(appEnvironment);