// <copyright file="ConventionsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Conventions
{
    using System.IO;
    using System.Threading.Tasks;
    using NDepend.Path;
    using Newtonsoft.Json;

    public class ConventionsManager
    {
        public async Task<ContentTypeConventions> LoadAsync(IAbsoluteFilePath path)
        {
            return path.Exists ?
                JsonConvert.DeserializeObject<ContentTypeConventions>(await File.ReadAllTextAsync(path.ToString()).ConfigureAwait(false)) : null;
        }

        public async Task SaveAsync(IAbsoluteFilePath path, ContentTypeConventions conventions)
        {
            string json = JsonConvert.SerializeObject(conventions, Formatting.Indented);

            await File.WriteAllTextAsync(path.ToString(), json).ConfigureAwait(false);
        }
    }
}