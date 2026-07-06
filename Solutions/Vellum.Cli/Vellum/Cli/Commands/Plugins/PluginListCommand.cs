// <copyright file="PluginListCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading;

using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Commands.Plugins;

public class PluginListCommand(IAppEnvironment appEnvironment) : Command
{
    protected override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        foreach (DirectoryPath pluginPath in appEnvironment.PluginPaths)
        {
            AnsiConsole.MarkupLineInterpolated($"-🔌{pluginPath.GetParent()?.GetDirectoryName()} ([green]{pluginPath.GetDirectoryName()}[/])");
        }

        return ReturnCodes.Ok;
    }
}