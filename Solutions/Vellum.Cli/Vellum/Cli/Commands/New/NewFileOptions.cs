// <copyright file="NewFileOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.New
{
    using System.IO;

    public class NewFileOptions
    {
        public NewFileOptions(DirectoryInfo output)
        {
            this.Output = output;
        }

        public DirectoryInfo Output { get; }
    }
}