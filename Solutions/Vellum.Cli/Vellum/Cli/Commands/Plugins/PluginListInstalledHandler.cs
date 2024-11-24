﻿// <copyright file="PluginListInstalledHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Plugins
{
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;

    using Spectre.Console;

    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Infrastructure;
    using Vellum.Cli.Abstractions.Plugins;
    using Vellum.Cli.Packages;

    public static class PluginListInstalledHandler
    {
        public static Task<int> ExecuteAsync(
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var packageManager = new NuGetPluginPackageManager(appEnvironment);

            console.Write(new Markup("Listing installed plugins:\n"));

            var table = new Table
            {
                Border = TableBorder.SimpleHeavy,
            };

            table.AddColumn(new TableColumn("Id"));
            table.AddColumn(new TableColumn("Version"));
            table.AddColumn(new TableColumn("Path"));

            foreach (PluginPackage package in packageManager.ListInstalledPackages())
            {
                table.AddRow(package.Name, package.Version, package.PluginPath.ToString());
            }

            console.Write(table);

            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}