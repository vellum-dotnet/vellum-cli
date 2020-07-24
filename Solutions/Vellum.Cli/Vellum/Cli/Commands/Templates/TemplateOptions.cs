// <copyright file="TemplateOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Templates
{
    public class TemplateOptions
    {
        public TemplateOptions(string packageId)
        {
            this.PackageId = packageId;
        }

        public string PackageId { get; }
    }
}