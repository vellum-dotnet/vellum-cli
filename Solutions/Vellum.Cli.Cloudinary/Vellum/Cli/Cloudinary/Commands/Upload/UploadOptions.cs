// <copyright file="UploadOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Cloudinary.Commands.Upload
{
    using System.IO;

    public class UploadOptions
    {
        public UploadOptions(FileInfo filePath)
        {
            this.FilePath = filePath;
        }

        public FileInfo FilePath { get; }
    }
}