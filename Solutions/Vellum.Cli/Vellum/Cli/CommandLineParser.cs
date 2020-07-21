// <copyright file="CommandLineParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Invocation;
    using System.CommandLine.Parsing;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Commands.Environment;
    using Vellum.Cli.Commands.Plugins;
    using Vellum.Cli.Commands.Templates;
    using Vellum.Cli.Plugins;

    public class CommandLineParser
    {
        private readonly IAppEnvironment appEnvironment;
        private readonly ICommandPluginHost commandPluginHost;
        private readonly IServiceCollection services;

        public CommandLineParser(IAppEnvironment appEnvironment, ICommandPluginHost commandPluginHost, IServiceCollection services)
        {
            this.services = services;
            this.commandPluginHost = commandPluginHost;
            this.appEnvironment = appEnvironment;
        }

        public delegate Task EnvironmentInit(EnvironmentOptions options, IConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task PluginInstall(PluginOptions options, IConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task PluginUninstall(PluginOptions options, IConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task PluginList(IConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task TemplateInstall(TemplateOptions options, IConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task TemplateUninstall(TemplateOptions options, IConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public Parser Create(
            EnvironmentInit environmentInit = null,
            PluginInstall pluginInstall = null,
            PluginUninstall pluginUninstall = null,
            PluginList pluginList = null,
            TemplateInstall templateInstall = null,
            TemplateUninstall templateUninstall = null)
        {
            // if environmentInit hasn't been provided (for testing) then assign the Command Handler
            environmentInit ??= EnvironmentInitHandler.ExecuteAsync;
            pluginInstall ??= PluginInstallHandler.ExecuteAsync;
            pluginUninstall ??= PluginUninstallHandler.ExecuteAsync;
            pluginList ??= PluginListHandler.ExecuteAsync;
            templateInstall ??= TemplatePackageInstallerHandler.ExecuteAsync;
            templateUninstall ??= TemplatePackageUninstallerHandler.ExecuteAsync;

            // Set up intrinsic commands that will always be available.
            RootCommand rootCommand = Root();
            rootCommand.AddCommand(Environment());
            rootCommand.AddCommand(Plugins());
            rootCommand.AddCommand(Templates());

            CommandLineBuilder commandBuilder = new CommandLineBuilder(rootCommand).UseDefaults();

            try
            {
                foreach (Command command in this.commandPluginHost.Discover(this.appEnvironment.PluginPaths))
                {
                    commandBuilder.AddCommand(command);
                }
            }
            catch (DirectoryNotFoundException)
            {
                // If this is the first run, initialize the environment and default plugins
                Console.WriteLine("Error Detected: vellum environment uninitialized.");
                Parser parser = commandBuilder.Build();

                int result = parser.Invoke("environment init");

                if (result == ReturnCodes.Ok)
                {
                    // Now the environment has been re-initialized, try to discover the plugins again.
                    foreach (Command command in this.commandPluginHost.Discover(this.appEnvironment.PluginPaths))
                    {
                        commandBuilder.AddCommand(command);
                    }
                }
            }

            return commandBuilder.Build();

            RootCommand Root()
            {
                return new RootCommand
                {
                    Name = "vellum",
                    Description = "Static Content Management System.",
                };
            }

            Command Environment()
            {
                var environmentCommand = new Command(
                    "environment",
                    "Manipulate the vellum-cli environment & settings.");

                var initCommand = new Command("init", "Initialize the environment & settings.")
                {
                    Handler = CommandHandler.Create<EnvironmentOptions, InvocationContext>(async (options, context) =>
                    {
                        await environmentInit(options, context.Console, this.appEnvironment, context);
                    }),
                };

                environmentCommand.AddCommand(initCommand);

                return environmentCommand;
            }

            Command Plugins()
            {
                var pluginsCmd = new Command(
                    "plugins",
                    "Manage vellum-cli plugins.");

                var installCmd = new Command("install", "Install a vellum-cli plugin.")
                {
                    new Argument<string>
                    {
                        Name = "PackageId",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                installCmd.Handler = CommandHandler.Create<PluginOptions, InvocationContext>(async (options, context) =>
                {
                    await pluginInstall(options, context.Console, this.appEnvironment, context);
                });

                var uninstallCmd = new Command("uninstall", "Uninstall a vellum-cli plugin.")
                {
                    new Argument<string>
                    {
                        Name = "PackageId",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                uninstallCmd.Handler = CommandHandler.Create<PluginOptions, InvocationContext>(async (options, context) =>
                {
                    await pluginUninstall(options, context.Console, this.appEnvironment, context);
                });

                var listCmd = new Command("list", "List installed vellum-cli plugins.")
                {
                    Handler = CommandHandler.Create<InvocationContext>(async (context) =>
                    {
                        await pluginList(context.Console, this.appEnvironment, context);
                    }),
                };

                pluginsCmd.AddCommand(installCmd);
                pluginsCmd.AddCommand(uninstallCmd);
                pluginsCmd.AddCommand(listCmd);

                return pluginsCmd;
            }

            Command Templates()
            {
                var cmd = new Command("templates", "Perform operations on Vellum templates.");

                var packagesCmd = new Command("packages", "Perform operations on Vellum template packages.");

                var installCmd = new Command("install", "Install a vellum-cli template package.")
                {
                    new Argument<string>
                    {
                        Name = "PackageId",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                installCmd.Handler = CommandHandler.Create<TemplateOptions, InvocationContext>(async (options, context) =>
                {
                    await templateInstall(options, context.Console, this.appEnvironment, context);
                });

                var uninstallCmd = new Command("uninstall", "Uninstall a vellum-cli template package.")
                {
                    new Argument<string>
                    {
                        Name = "PackageId",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                uninstallCmd.Handler = CommandHandler.Create<TemplateOptions, InvocationContext>(async (options, context) =>
                {
                    await templateUninstall(options, context.Console, this.appEnvironment, context);
                });

                packagesCmd.AddCommand(installCmd);
                packagesCmd.AddCommand(uninstallCmd);

                cmd.AddCommand(packagesCmd);

                return cmd;
            }
        }
    }
}