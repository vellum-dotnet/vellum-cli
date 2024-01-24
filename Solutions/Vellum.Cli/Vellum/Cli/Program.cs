// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

using Vellum.Cli.Abstractions.Commands;
using Vellum.Cli.Commands.Environment;
using Vellum.Cli.Commands.Plugins;
using Vellum.Cli.Commands.Templates;
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
            config.SetApplicationName("vellum-cli");
            config.Settings.PropagateExceptions = false;
            config.CaseSensitivity(CaseSensitivity.None);
            config.ValidateExamples();

            config.AddBranch("environment", environment =>
            {
                environment.SetDescription("Manipulate the vellum-cli environment & settings.");

                environment.AddCommand<EnvironmentInitCommand>("init")
                    .WithDescription("Initialize the vellum-cli environment.");
                environment.AddCommand<SetEnvironmentSettingCommand>("set")
                    .WithDescription("Set a vellum-cli environment setting.");
            });

            // new command
            config.AddBranch("plugins", plugins =>
            {
                plugins.SetDescription("Manage vellum-cli plugins.");

                plugins.AddCommand<PluginClearCommand>("clear")
                    .WithDescription("Clear all vellum-cli plugins.");
                plugins.AddCommand<PluginInstallCommand>("install")
                    .WithDescription("Install a vellum-cli plugin.");
                plugins.AddCommand<PluginListCommand>("list")
                    .WithDescription("List installed vellum-cli plugins.");
                plugins.AddCommand<PluginUninstallCommand>("uninstall")
                    .WithDescription("Uninstall a vellum-cli plugin.");
            });

            config.AddBranch("templates", templates =>
            {
                templates.SetDescription("Perform operations on Vellum templates.");

                templates.AddBranch("packages", packages =>
                {
                    packages.SetDescription("Perform operations on Vellum template packages.");

                    packages.AddCommand<TemplatePackageInstallerCommand>("install")
                        .WithDescription("Install a vellum-cli template package.");
                    packages.AddCommand<UninstallTemplateCommand>("uninstall")
                        .WithDescription("Uninstall a vellum-cli template package.");
                });
            });

            foreach (ICommandPlugin plugin in plugins)
            {
                plugin.Configure(config);
            }
        });

        return app.RunAsync(args);
    }
}