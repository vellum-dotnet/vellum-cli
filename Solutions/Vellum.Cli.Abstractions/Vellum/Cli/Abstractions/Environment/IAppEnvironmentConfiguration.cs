// <copyright file="IAppEnvironmentConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.IO;

namespace Vellum.Cli.Abstractions.Environment;

public interface IAppEnvironmentConfiguration
{
    DirectoryPath AppPath { get; }

    DirectoryPath ConfigurationPath { get; }
}