// <copyright file="ConventionsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spectre.IO;

namespace Vellum.Cli.Abstractions.Conventions;

public static class ConventionsManager
{
    public static async Task<ContentTypeConventionsRoot> LoadAsync(FilePath path)
    {
        return System.IO.Path.Exists(path.FullPath) ?
            JsonConvert.DeserializeObject<ContentTypeConventionsRoot>(await File.ReadAllTextAsync(path.ToString()).ConfigureAwait(false)) : null;
    }

    public static async Task SaveAsync(FilePath path, ContentTypeConventionsRoot conventionsRoot)
    {
        string json = JsonConvert.SerializeObject(conventionsRoot, Formatting.Indented);

        await File.WriteAllTextAsync(path.ToString(), json).ConfigureAwait(false);
    }
}