// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

using Vellum.Cli.Abstractions.Commands;
using Vellum.Cli.Environment;
using Vellum.Cli.Infrastructure;
using Vellum.Cli.Infrastructure.Injection;
using Vellum.Cli.Plugins;

namespace Vellum.Cli;

public static class Program
{
    public static Task<int> Main(string[] args)
    {
        CommandPluginHost pluginHost = new();
        FileSystemRoamingProfileAppEnvironment appEnvironment = new();

        IEnumerable<ICommandPlugin> plugins = pluginHost.Discover(appEnvironment.PluginPaths);

        ServiceCollection registrations = [];
        registrations.ConfigureDependencies();

        TypeRegistrar registrar = new(registrations);
        CommandApp app = new(registrar);

        app.Configure(config =>
        {
            config.Settings.PropagateExceptions = false;
            config.CaseSensitivity(CaseSensitivity.None);
            config.SetApplicationName("vellum");
            config.ValidateExamples();

            foreach (ICommandPlugin plugin in plugins)
            {
                plugin.Configure(config);
            }
        });

        return app.RunAsync(args);
    }
}