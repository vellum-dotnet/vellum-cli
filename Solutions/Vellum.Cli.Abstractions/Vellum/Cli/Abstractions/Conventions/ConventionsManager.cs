// <copyright file="ConventionsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Spectre.IO;

namespace Vellum.Cli.Abstractions.Conventions;

public static class ConventionsManager
{
    public static async Task<ContentTypeConventionsRoot> LoadAsync(FilePath path)
    {
        return System.IO.Path.Exists(path.FullPath) ?
            JsonSerializer.Deserialize<ContentTypeConventionsRoot>(await File.ReadAllTextAsync(path.ToString()).ConfigureAwait(false)) : null;
    }

    public static async Task SaveAsync(FilePath path, ContentTypeConventionsRoot conventionsRoot)
    {
        string json = JsonSerializer.Serialize(conventionsRoot, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(path.ToString(), json).ConfigureAwait(false);
    }
}