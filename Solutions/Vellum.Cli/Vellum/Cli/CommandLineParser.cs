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
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Commands.Environment;
    using Vellum.Cli.Commands.Plugins;
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

        public delegate Task PluginList(IConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public Parser Create(
            EnvironmentInit environmentInit = null,
            PluginInstall pluginInstall = null,
            PluginList pluginList = null)
        {
            // if environmentInit hasn't been provided (for testing) then assign the Command Handler
            environmentInit ??= EnvironmentInitHandler.ExecuteAsync;
            pluginInstall ??= PluginInstallHandler.ExecuteAsync;
            pluginList ??= PluginListHandler.ExecuteAsync;

            // Set up intrinsic commands that will always be available.
            RootCommand rootCommand = Root();
            rootCommand.AddCommand(Environment());
            rootCommand.AddCommand(Plugins());

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

                if (result == 0)
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
                    new Option("--package-id", "NuGet Package Id")
                    {
                        Argument = new Argument<string>(),
                    },
                };

                installCmd.Handler = CommandHandler.Create<PluginOptions, InvocationContext>(async (options, context) =>
                {
                    await pluginInstall(options, context.Console, this.appEnvironment, context);
                });

                var listCmd = new Command("list", "List installed vellum-cli plugins.")
                {
                    Handler = CommandHandler.Create<InvocationContext>(async (context) =>
                    {
                        await pluginList(context.Console, this.appEnvironment, context);
                    }),
                };

                pluginsCmd.AddCommand(installCmd);
                pluginsCmd.AddCommand(listCmd);

                return pluginsCmd;
            }
        }
    }
}