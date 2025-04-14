// <copyright file="IFileReader{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading.Tasks;

using NDepend.Path;

namespace Vellum.Abstractions.IO;

public interface IFileReader<T>
{
    string ContentType { get; }

    Task<T> ReadAsync(IAbsoluteFilePath filePath);
}