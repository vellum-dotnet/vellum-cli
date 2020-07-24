// <copyright file="EnvironmentOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.IO;

    public class EnvironmentOptions
    {
        public EnvironmentOptions(DirectoryInfo output)
        {
            this.Output = output;
        }

        public DirectoryInfo Output { get; }
    }
}