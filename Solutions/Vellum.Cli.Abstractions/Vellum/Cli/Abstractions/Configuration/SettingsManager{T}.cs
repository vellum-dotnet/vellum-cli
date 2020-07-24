// <copyright file="SettingsManager{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Configuration
{
    using System.IO;
    using NDepend.Path;
    using Newtonsoft.Json;
    using Vellum.Cli.Abstractions.Environment;

    public class SettingsManager<T> : ISettingsManager<T>
        where T : class
    {
        private readonly IAppEnvironmentConfiguration appEnvironment;

        public SettingsManager(IAppEnvironmentConfiguration appEnvironment)
        {
            this.appEnvironment = appEnvironment;
        }

        public T LoadSettings()
        {
            string filePath = $"{this.GetLocalFilePath(typeof(T).Name)}.json";

            return File.Exists(filePath) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath)) : null;
        }

        public void SaveSettings(T settings)
        {
            IAbsoluteFilePath filePath = this.GetLocalFilePath(typeof(T).Name);
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

            File.WriteAllText($"{filePath}.json", json);
        }

        private IAbsoluteFilePath GetLocalFilePath(string fileName)
        {
            return this.appEnvironment.ConfigurationPath.GetChildFileWithName(fileName);
        }
    }
}