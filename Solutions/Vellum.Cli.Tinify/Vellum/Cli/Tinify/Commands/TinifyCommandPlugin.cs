// <copyright file="TinifyCommandPlugin.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.Console.Cli;
using Vellum.Cli.Abstractions.Commands;
using Vellum.Cli.Tinify.Commands.Optimize;
using Vellum.Cli.Tinify.Commands.Settings;

namespace Vellum.Cli.Tinify.Commands;

public class TinifyCommandPlugin : ICommandPlugin
{
    public void Configure(IConfigurator configurator)
    {
        configurator.AddBranch("tinify", root =>
        {
            root.SetDescription("Optimize media assets with Tinify.");

            root.AddBranch("settings", settings =>
            {
                settings.SetDescription("Manage Tinify settings.");
                settings.AddCommand<ListCommand>("list")
                        .WithDescription("List Tinify settings.");
                settings.AddCommand<UpdateCommand>("update")
                        .WithDescription("Update Tinify settings.");
            });
            root.AddCommand<OptimizeCommand>("optimize")
                .WithDescription("Optimize images using Tinify.");
        });
    }
}