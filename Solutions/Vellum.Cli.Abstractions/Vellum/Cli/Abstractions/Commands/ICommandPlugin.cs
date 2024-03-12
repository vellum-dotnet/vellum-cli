// <copyright file="ICommandPlugin.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.Console.Cli;

namespace Vellum.Cli.Abstractions.Commands;

public interface ICommandPlugin
{
    void Configure(IConfigurator configurator);
}