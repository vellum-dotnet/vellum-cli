// <copyright file="ConventionsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Conventions
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using NDepend.Path;
    using Newtonsoft.Json;

    public class ConventionsManager
    {
        public async Task<ContentTypeConventionsRoot> LoadAsync(IAbsoluteFilePath path)
        {
            ArgumentNullException.ThrowIfNull(path);

            return path.Exists ? JsonConvert.DeserializeObject<ContentTypeConventionsRoot>(await File.ReadAllTextAsync(path.ToString()).ConfigureAwait(false)) : null;
        }

        public async Task SaveAsync(IAbsoluteFilePath path, ContentTypeConventionsRoot conventionsRoot)
        {
            ArgumentNullException.ThrowIfNull(path);

            string json = JsonConvert.SerializeObject(conventionsRoot, Formatting.Indented);

            await File.WriteAllTextAsync(path.ToString(), json).ConfigureAwait(false);
        }
    }
}