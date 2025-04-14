// <copyright file="YamlParser{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.IO;
using System.Threading.Tasks;

using NDepend.Path;

using YamlDotNet.Serialization;

namespace Vellum.Abstractions.Parsers;

public class YamlParser<T>
{
    public async Task<T> ParseAsync(IAbsoluteFilePath filePath)
    {
        IDeserializer deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();
        string content = await File.ReadAllTextAsync(filePath.ToString()).ConfigureAwait(continueOnCapturedContext: false);

        return deserializer.Deserialize<T>(content);
    }
}