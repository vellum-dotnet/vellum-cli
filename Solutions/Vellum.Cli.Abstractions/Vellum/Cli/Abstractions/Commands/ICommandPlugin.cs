// <copyright file="ICommandPlugin.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Commands
{
    using System.CommandLine;

    public interface ICommandPlugin
    {
        Command Command();
    }
}
