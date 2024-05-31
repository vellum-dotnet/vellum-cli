// <copyright file="SettingsManager{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.IO;
using System.Text.Json;

using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Abstractions.Configuration;

public class SettingsManager<T> : ISettingsManager<T>
    where T : class
{
    private readonly IAppEnvironmentConfiguration appEnvironmentConfiguration;

    public SettingsManager(IAppEnvironmentConfiguration appEnvironmentConfiguration)
    {
        this.appEnvironmentConfiguration = appEnvironmentConfiguration;
    }

    public T LoadSettings(string fileName)
    {
        string filePath = $"{this.GetLocalFilePath(fileName)}.json";

        return File.Exists(filePath) ? JsonSerializer.Deserialize<T>(File.ReadAllText(filePath)) : null;
    }

    public void SaveSettings(T settings, string fileName)
    {
        string filePath = this.GetLocalFilePath(fileName);
        string json = JsonSerializer.Serialize(settings);

        File.WriteAllText($"{filePath}.json", json);
    }

    private string GetLocalFilePath(string fileName)
    {
        return Path.Combine(this.appEnvironmentConfiguration.ConfigurationPath.ToString(), fileName);
    }
}