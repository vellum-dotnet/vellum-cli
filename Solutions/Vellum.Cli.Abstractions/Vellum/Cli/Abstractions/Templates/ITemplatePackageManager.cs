// <copyright file="ITemplatePackageManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace Vellum.Cli.Abstractions.Templates;

public interface ITemplatePackageManager
{
    Task<TemplatePackage> InstallLatestAsync(string packageId);
}