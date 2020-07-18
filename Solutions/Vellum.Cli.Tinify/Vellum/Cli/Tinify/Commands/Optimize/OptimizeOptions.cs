// <copyright file="OptimizeOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Tinify.Commands.Optimize
{
    using System.IO;

    public class OptimizeOptions
    {
        public OptimizeOptions(FileInfo filePath)
        {
            this.FilePath = filePath;
        }

        public FileInfo FilePath { get; }
    }
}