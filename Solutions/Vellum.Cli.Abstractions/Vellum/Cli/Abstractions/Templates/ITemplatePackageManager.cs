// <copyright file="ITemplatePackageManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Templates
{
    using System.Threading.Tasks;

    public interface ITemplatePackageManager
    {
        Task<TemplatePackage> InstallLatestAsync(string packageId);
    }
}