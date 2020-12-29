// <copyright file="IFileReader{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.IO
{
    using System.Threading.Tasks;

    using NDepend.Path;

    public interface IFileReader<T>
    {
        string ContentType { get; }

        Task<T> ReadAsync(IAbsoluteFilePath filePath);
    }
}
