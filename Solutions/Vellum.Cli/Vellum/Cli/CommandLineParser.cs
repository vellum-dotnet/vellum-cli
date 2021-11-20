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

    using NDepend.Path;

    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Commands.Content;
    using Vellum.Cli.Commands.Environment;
    using Vellum.Cli.Commands.New;
    using Vellum.Cli.Commands.Plugins;
    using Vellum.Cli.Commands.Templates;
    using Vellum.Cli.Plugins;

    public class CommandLineParser
    {
        private readonly ICompositeConsole console;
        private readonly IAppEnvironment appEnvironment;
        private readonly ICommandPluginHost commandPluginHost;
        private readonly IServiceCollection services;

        public CommandLineParser(ICompositeConsole console, IAppEnvironment appEnvironment, ICommandPluginHost commandPluginHost, IServiceCollection services)
        {
            this.console = console;
            this.services = services;
            this.commandPluginHost = commandPluginHost;
            this.appEnvironment = appEnvironment;
        }

        public delegate Task ContentList(IServiceCollection services, ListOptions options, ICompositeConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task EnvironmentInit(EnvironmentOptions options, ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task NewFile(NewFileOptions fileOptions, ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task PluginInstall(PluginOptions options, ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task PluginUninstall(PluginOptions options, ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task PluginList(ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task SetEnvironmentSetting(SetOptions options, ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task TemplateInstall(TemplateOptions options, ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public delegate Task TemplateUninstall(TemplateOptions options, ICompositeConsole console,  IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

        public Parser Create(
            ContentList contentList = null,
            EnvironmentInit environmentInit = null,
            NewFile newFile = null,
            PluginInstall pluginInstall = null,
            PluginUninstall pluginUninstall = null,
            PluginList pluginListInstalled = null,
            SetEnvironmentSetting setEnvironmentSetting = null,
            TemplateInstall templateInstall = null,
            TemplateUninstall templateUninstall = null)
        {
            // if environmentInit hasn't been provided (for testing) then assign the Command Handler
            contentList ??= ContentListHandler.ExecuteAsync;
            environmentInit ??= EnvironmentInitHandler.ExecuteAsync;
            newFile ??= NewFileHandler.ExecuteAsync;
            pluginInstall ??= PluginInstallHandler.ExecuteAsync;
            pluginUninstall ??= PluginUninstallHandler.ExecuteAsync;
            pluginListInstalled ??= PluginListInstalledHandler.ExecuteAsync;
            setEnvironmentSetting ??= SetEnvironmentSettingHandler.ExecuteAsync;
            templateInstall ??= TemplatePackageInstallerHandler.ExecuteAsync;
            templateUninstall ??= TemplatePackageUninstallerHandler.ExecuteAsync;

            // Set up intrinsic commands that will always be available.
            RootCommand rootCommand = Root();
            rootCommand.AddCommand(Content());
            rootCommand.AddCommand(Environment());
            rootCommand.AddCommand(NewFile());
            rootCommand.AddCommand(Plugins());
            rootCommand.AddCommand(Templates());

            var commandBuilder = new CommandLineBuilder(rootCommand);

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
                Parser parser = commandBuilder.UseDefaults().Build();

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

            return commandBuilder.UseDefaults().Build();

            static RootCommand Root()
            {
                return new RootCommand
                {
                    Name = "vellum-cli",
                    Description = "Static Content Management System.",
                };
            }

            Command Content()
            {
                var cmd = new Command(
                    "content",
                    "Edit, preview, publish and promote content.");

                var listCmd = new Command("list", "List content in your repository.")
                {
                    Handler = CommandHandler.Create<ListOptions, InvocationContext>(async (options, context) =>
                    {
                        await contentList(this.services, options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                    }),
                };

                listCmd.AddArgument(new Argument<DirectoryInfo>
                {
                    Name = "site-taxonomy-directory-path",
                    Description = "Path to the site template directory.",
                    Arity = ArgumentArity.ZeroOrOne,
                }.ExistingOnly());

                listCmd.AddOption(new Option<bool>("--published", "Show only published content."));
                listCmd.AddOption(new Option<bool>("--draft", "Show only draft content."));

                cmd.AddCommand(listCmd);

                return cmd;
            }

            Command Environment()
            {
                var cmd = new Command(
                    "environment",
                    "Manipulate the vellum-cli environment & settings.");

                var initCmd = new Command("init", "Initialize the environment & settings.")
                {
                    Handler = CommandHandler.Create<EnvironmentOptions, InvocationContext>(async (options, context) =>
                    {
                        await environmentInit(options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                    }),
                };

                var setCmd = new Command(
                    "set",
                    "Set vellum-cli environment configuration.");

                setCmd.AddOption(new Option("--username", "Username for the current user.")
                {
                    Argument =  new Argument<string>
                    {
                        Arity = ArgumentArity.ExactlyOne,
                    },
                });

                setCmd.AddOption(new Option("--workspace-path", "The location of your vellum workspace.")
                {
                    Argument =  new Argument<DirectoryInfo>
                    {
                        Arity = ArgumentArity.ExactlyOne,
                    },
                });

                setCmd.AddOption(new Option("--publish-path", "The location for generated output.")
                {
                    Argument =  new Argument<DirectoryInfo>
                    {
                        Arity = ArgumentArity.ExactlyOne,
                    },
                });

                setCmd.AddOption(new Option("--key", "A user-defined setting key.")
                {
                    Argument =  new Argument<string>
                    {
                        Arity = ArgumentArity.ExactlyOne,
                    },
                });

                setCmd.AddOption(new Option("--value", "A user-defined setting value for the specified key.")
                {
                    Argument =  new Argument<string>
                    {
                        Arity = ArgumentArity.ExactlyOne,
                    },
                });

               // System.CommandLine doesn't support mutually inclusive fileOptions, so you need to enforce this behaviour with a validator.
/*
               ValueForOption has been removed see https://github.com/dotnet/command-line-api/issues/1119
               setCmd.AddValidator(commandResult =>
                {
                    DirectoryInfo workspace = commandResult.ValueForOption <DirectoryInfo>("workspace-path");
                    DirectoryInfo publish = commandResult.ValueForOption<DirectoryInfo>("publish-path");
                    string username = commandResult.ValueForOption<string>("username");
                    string key = commandResult.ValueForOption<string>("key");
                    string value = commandResult.ValueForOption<string>("value");

                    if (workspace == null && publish == null && username == null && key == null && value == null)
                    {
                        return "Please specify at least one option.";
                    }

                    if ((key != null && value == null) || (key == null && value != null))
                    {
                        return "--key & --value are mutually inclusive. Please specify a value for --key AND --value";
                    }

                    return null;
                });
*/

                setCmd.Handler = CommandHandler.Create<SetOptions, InvocationContext>(async (options, context) =>
                {
                    await setEnvironmentSetting(options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                });

                cmd.AddCommand(initCmd);
                cmd.AddCommand(setCmd);

                return cmd;
            }

            Command NewFile()
            {
                var cmd = new Command(
                    "new",
                    "Create new files based on templates.")
                {
                    new Argument<string>("template-name")
                    {
                        Description = "Name of the template, as defined by the template convention",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                    new Argument<FileInfo>("file-path")
                    {
                        Description = "Where do you want the new file to be created?",
                        Arity = ArgumentArity.ZeroOrOne,
                    },
                };

                cmd.Handler = CommandHandler.Create<NewFileOptions, InvocationContext>(async (options, context) =>
                {
                    await newFile(options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                });

                return cmd;
            }

            Command Plugins()
            {
                var cmd = new Command(
                    "plugins",
                    "Manage vellum-cli plugins.");

                var installCmd = new Command("install", "Install a vellum-cli plugin.")
                {
                    new Argument<string>
                    {
                        Name = "package-id",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                installCmd.Handler = CommandHandler.Create<PluginOptions, InvocationContext>(async (options, context) =>
                {
                    await pluginInstall(options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                });

                var uninstallCmd = new Command("uninstall", "Uninstall a vellum-cli plugin.")
                {
                    new Argument<string>
                    {
                        Name = "package-id",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                uninstallCmd.Handler = CommandHandler.Create<PluginOptions, InvocationContext>(async (options, context) =>
                {
                    await pluginUninstall(options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                });

                var listCmd = new Command("list", "List vellum-cli plugins.")
                {
                };

                var listInstalledCmd = new Command("installed", "List installed vellum-cli plugins.")
                {
                    Handler = CommandHandler.Create<InvocationContext>(async (context) =>
                    {
                        await pluginListInstalled(this.console, this.appEnvironment, context).ConfigureAwait(false);
                    }),
                };

                listCmd.AddCommand(listInstalledCmd);

                cmd.AddCommand(installCmd);
                cmd.AddCommand(uninstallCmd);
                cmd.AddCommand(listCmd);

                return cmd;
            }

            Command Templates()
            {
                var cmd = new Command("templates", "Perform operations on Vellum templates.");

                var packagesCmd = new Command("packages", "Perform operations on Vellum template packages.");

                var installCmd = new Command("install", "Install a vellum-cli template package.")
                {
                    new Argument<string>
                    {
                        Name = "package-id",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                installCmd.Handler = CommandHandler.Create<TemplateOptions, InvocationContext>(async (options, context) =>
                {
                    await templateInstall(options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                });

                var uninstallCmd = new Command("uninstall", "Uninstall a vellum-cli template package.")
                {
                    new Argument<string>
                    {
                        Name = "package-id",
                        Description = "NuGet Package Id",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                uninstallCmd.Handler = CommandHandler.Create<TemplateOptions, InvocationContext>(async (options, context) =>
                {
                    await templateUninstall(options, this.console, this.appEnvironment, context).ConfigureAwait(false);
                });

                packagesCmd.AddCommand(installCmd);
                packagesCmd.AddCommand(uninstallCmd);

                cmd.AddCommand(packagesCmd);

                return cmd;
            }
        }
    }
}