// <copyright file="YamlParser{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.IO;
using System.Threading.Tasks;

using Spectre.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Vellum.Abstractions.Parsers;

public class YamlParser<T>
{
    public async Task<T> ParseAsync(FilePath filePath)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .WithCaseInsensitivePropertyMatching()
            .IgnoreUnmatchedProperties()
            .IgnoreFields()
            .Build();

        string content = await File.ReadAllTextAsync(filePath.ToString()!).ConfigureAwait(continueOnCapturedContext: false);

        return deserializer.Deserialize<T>(content);
    }
}