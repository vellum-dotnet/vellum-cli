// <copyright file="NewFileOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.New
{
    using System.IO;

    public class NewFileOptions
    {
        public NewFileOptions(string templateName, FileInfo filePath)
        {
            this.TemplateName = templateName;
            this.FilePath = filePath;
        }

        public string TemplateName { get; }

        public FileInfo FilePath { get; }
    }
}