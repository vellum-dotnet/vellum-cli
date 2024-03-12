// <copyright file="TemplateSettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Vellum.Cli.Abstractions.Configuration;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Abstractions.Templates;

public class TemplateSettingsManager : SettingsManager<TemplatesSettings>
{
    public TemplateSettingsManager(IAppEnvironment appEnvironment)
        : base(appEnvironment)
    {
        }
}