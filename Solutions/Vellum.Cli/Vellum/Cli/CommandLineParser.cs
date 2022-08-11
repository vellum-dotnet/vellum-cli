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
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.Extensions.DependencyInjection;

  using Vellum.Cli.Abstractions;
  using Vellum.Cli.Abstractions.Environment;
  using Vellum.Cli.Commands.Environment;
  using Vellum.Cli.Commands.New;
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

    public delegate Task EnvironmentInit(IConsole console, IAppEnvironment appEnvironment);

    public delegate Task NewFile(string templateName, FileInfo filePath, IConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

    public delegate Task PluginInstall(string packageId, IConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

    public delegate Task PluginUninstall(string packageId, IConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

    public delegate Task PluginList(IConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

    public delegate Task SetEnvironmentSettings(string username, DirectoryInfo workspacePath, DirectoryInfo publishPath, string key, string value, IConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

    public delegate Task TemplateInstall(string packageId, IConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

    public delegate Task TemplateUninstall(string packageId, IConsole console, IAppEnvironment appEnvironment, InvocationContext invocationContext = null);

    public Parser Create(
        EnvironmentInit environmentInit = null,
        NewFile newFile = null,
        PluginInstall pluginInstall = null,
        PluginUninstall pluginUninstall = null,
        PluginList pluginList = null,
        SetEnvironmentSettings setEnvironmentSettings = null,
        TemplateInstall templateInstall = null,
        TemplateUninstall templateUninstall = null)
    {
      // if environmentInit hasn't been provided (for testing) then assign the Command Handler
      environmentInit ??= EnvironmentInitHandler.ExecuteAsync;
      newFile ??= NewFileHandler.ExecuteAsync;
      pluginInstall ??= PluginInstallHandler.ExecuteAsync;
      pluginUninstall ??= PluginUninstallHandler.ExecuteAsync;
      pluginList ??= PluginListHandler.ExecuteAsync;
      setEnvironmentSettings ??= SetEnvironmentSettingHandler.ExecuteAsync;
      templateInstall ??= TemplatePackageInstallerHandler.ExecuteAsync;
      templateUninstall ??= TemplatePackageUninstallerHandler.ExecuteAsync;

      // Set up intrinsic commands that will always be available.
      RootCommand rootCommand = Root();
      rootCommand.AddCommand(Environment());
      rootCommand.AddCommand(NewFile());
      rootCommand.AddCommand(Plugins());
      rootCommand.AddCommand(Templates());

      var commandBuilder = new CommandLineBuilder(rootCommand);

      try
      {
        foreach (Command command in this.commandPluginHost.Discover(this.appEnvironment.PluginPaths))
        {
          commandBuilder.Command.AddCommand(command);
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
            commandBuilder.Command.AddCommand(command);
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

      Command Environment()
      {
        var cmd = new Command(
            "environment",
            "Manipulate the vellum-cli environment & settings.");

        cmd.AddCommand(InitCommand());
        cmd.AddCommand(SetCommand());

        return cmd;

        Command InitCommand()
        {
          var initCmd = new Command("init", "Initialize the environment & settings.");

          initCmd.SetHandler(async (context) => await environmentInit(context.Console, this.appEnvironment).ConfigureAwait(false));

          return initCmd;
        }

        Command SetCommand()
        {
          var setCmd = new Command(
                      "set",
                      "Set vellum-cli environment configuration.");

          var usernameOption = new Option<string>("--username", "Username for the current user.")
          {
            Arity = ArgumentArity.ExactlyOne,
          };

          var workspacePathOption = new Option<DirectoryInfo>("--workspace-path", "The location of your vellum workspace.")
          {
            Arity = ArgumentArity.ExactlyOne,
          };

          var publishPathOption = new Option<DirectoryInfo>("--publish-path", "The location for generated output.")
          {
            Arity = ArgumentArity.ExactlyOne,
          };

          var keyOption = new Option<string>("--key", "A user-defined setting key.")
          {
            Arity = ArgumentArity.ExactlyOne,
          };

          var valueOption = new Option<string>("--value", "A user-defined setting value for the specified key.")
          {
            Arity = ArgumentArity.ExactlyOne,
          };

          setCmd.Add(usernameOption);
          setCmd.Add(workspacePathOption);
          setCmd.Add(publishPathOption);
          setCmd.Add(keyOption);
          setCmd.Add(valueOption);

          /*
                  // System.CommandLine doesn't support mutually inclusive fileOptions, so you need to enforce this behaviour with a validator.
                  setCmd.AddValidator(commandResult =>
                      {
                        DirectoryInfo workspace = commandResult["workspace-path"].GetValueOrDefault<DirectoryInfo>();
                        DirectoryInfo publish = commandResult["publish-path"].GetValueOrDefault<DirectoryInfo>();
                        string username = commandResult["username"].GetValueOrDefault<string>();
                        string key = commandResult["key"].GetValueOrDefault<string>();
                        string value = commandResult["value"].GetValueOrDefault<string>();

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
          setCmd.SetHandler(async (context) =>
          {
            string username = context.ParseResult.GetValueForOption(usernameOption);
            DirectoryInfo workspacePath = context.ParseResult.GetValueForOption(workspacePathOption);
            DirectoryInfo publishPath = context.ParseResult.GetValueForOption(publishPathOption);
            string key = context.ParseResult.GetValueForOption(keyOption);
            string value = context.ParseResult.GetValueForOption(valueOption);

            await setEnvironmentSettings(username, workspacePath, publishPath, key, value, context.Console, this.appEnvironment, context).ConfigureAwait(false);
          });

          return setCmd;
        }
      }

      Command NewFile()
      {
        var cmd = new Command("new", "Create new files based on templates.");

        var templateNameArg = new Argument<string>("template-name")
        {
          Description = "Name of the template, as defined by the template convention",
          Arity = ArgumentArity.ExactlyOne,
        };

        var filePathArg = new Argument<FileInfo>("file-path")
        {
          Description = "Where do you want the new file to be created?",
          Arity = ArgumentArity.ZeroOrOne,
        };

        cmd.Add(templateNameArg);
        cmd.Add(filePathArg);

        cmd.SetHandler(async (context) =>
        {
          string templateName = context.ParseResult.GetValueForArgument(templateNameArg);
          FileInfo filePath = context.ParseResult.GetValueForArgument(filePathArg);

          await newFile(templateName, filePath, context.Console, this.appEnvironment, context).ConfigureAwait(false);
        });

        return cmd;
      }

      Command Plugins()
      {
        var cmd = new Command(
            "plugins",
            "Manage vellum-cli plugins.");

        cmd.AddCommand(Install());
        cmd.AddCommand(Uninstall());
        cmd.AddCommand(List());

        return cmd;

        Command Install()
        {
            var cmd = new Command("install", "Install a vellum-cli plugin.");

            var arg = new Argument<string>
            {
                Name = "package-id",
                Description = "NuGet Package Id",
                Arity = ArgumentArity.ExactlyOne,
            };

            cmd.Add(arg);

            cmd.SetHandler(async (context) =>
            {
                string packageId = context.ParseResult.GetValueForArgument(arg);
                await pluginInstall(packageId, context.Console, this.appEnvironment, context).ConfigureAwait(false);
            });

            return cmd;
        }

        Command Uninstall()
        {
            var cmd = new Command("uninstall", "Uninstall a vellum-cli plugin.");

            var arg = new Argument<string>
            {
                Name = "package-id",
                Description = "NuGet Package Id",
                Arity = ArgumentArity.ExactlyOne,
            };

            cmd.Add(arg);

            cmd.SetHandler(async (context) =>
            {
                string packageId = context.ParseResult.GetValueForArgument(arg);
                await pluginUninstall(packageId, context.Console, this.appEnvironment, context).ConfigureAwait(false);
            });

            return cmd;
        }

        Command List()
        {
            var cmd = new Command("list", "List installed vellum-cli plugins.");

            cmd.SetHandler(async (context) =>
            {
                await pluginList(context.Console, this.appEnvironment, context).ConfigureAwait(false);
            });

            return cmd;
        }
      }

      Command Templates()
      {
        var cmd = new Command("templates", "Perform operations on Vellum templates.");

        cmd.AddCommand(TemplatesPackages());

        return cmd;

        Command TemplatesPackages()
        {
            var cmd = new Command("packages", "Perform operations on Vellum template packages.");

            cmd.AddCommand(Install());
            cmd.AddCommand(Uninstall());

            return cmd;

            Command Install()
            {
                var cmd = new Command("install", "Install a vellum-cli template package.");

                var arg = new Argument<string>
                {
                    Name = "package-id",
                    Description = "NuGet Package Id",
                    Arity = ArgumentArity.ExactlyOne,
                };

                cmd.Add(arg);

                cmd.SetHandler(async (context) =>
                {
                    string packageId = context.ParseResult.GetValueForArgument(arg);
                    await templateInstall(packageId, context.Console, this.appEnvironment, context).ConfigureAwait(false);
                });

                return cmd;
            }

            Command Uninstall()
            {
                var cmd = new Command("uninstall", "Uninstall a vellum-cli template package.");

                var arg = new Argument<string>
                {
                    Name = "package-id",
                    Description = "NuGet Package Id",
                    Arity = ArgumentArity.ExactlyOne,
                };

                cmd.Add(arg);

                cmd.SetHandler(async (context) =>
                {
                    string packageId = context.ParseResult.GetValueForArgument(arg);
                    await templateUninstall(packageId, context.Console, this.appEnvironment, context).ConfigureAwait(false);
                });

                return cmd;
            }
        }
      }
    }
  }
}