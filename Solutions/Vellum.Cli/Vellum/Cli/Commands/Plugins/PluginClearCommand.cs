// <copyright file="PluginClearCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;

using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Commands.Plugins;

public class PluginClearCommand(IAppEnvironment appEnvironment) : Command
{
    public override int Execute(CommandContext context)
    {
        IEnumerable<DirectoryPath> paths = appEnvironment.PluginPaths;

        foreach (DirectoryPath directoryPath in paths)
        {
            DirectoryInfo di = new(directoryPath.FullPath);
            AnsiConsole.MarkupLineInterpolated($"❌ DELETED: [red]{di?.Parent?.FullName}[/]");
            di?.Parent?.Delete(recursive: true);
        }

        return ReturnCodes.Ok;
    }
}