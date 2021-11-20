// <copyright file="ICompositeConsole.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli
{
    using System.CommandLine;
    using Spectre.Console;

    public interface ICompositeConsole : IConsole, IAnsiConsole
    {
    }
}