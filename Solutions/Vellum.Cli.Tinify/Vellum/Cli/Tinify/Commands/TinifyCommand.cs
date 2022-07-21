// <copyright file="TinifyCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Tinify.Commands
{
  using System.CommandLine;
  using System.CommandLine.Invocation;
  using System.IO;
  using System.Threading.Tasks;

  using Vellum.Cli.Abstractions.Commands;
  using Vellum.Cli.Tinify.Commands.Optimize;
  using Vellum.Cli.Tinify.Commands.Settings;

  public class TinifyCommand : ICommandPlugin
  {
    private List list;
    private Update update;
    private Optimize optimizeAsync;

    public TinifyCommand()
    {
    }

    public TinifyCommand(List list, Update update, Optimize optimizeAsync)
    {
      this.list = list;
      this.update = update;
      this.optimizeAsync = optimizeAsync;
    }

    public delegate Task List(IConsole console, InvocationContext invocationContext = null);

    public delegate Task Optimize(FileInfo fileInfo, IConsole console, InvocationContext invocationContext = null);

    public delegate Task Update(string key, IConsole console, InvocationContext invocationContext = null);

    public Command Command()
    {
      this.list ??= ListCommandHandler.Execute;
      this.update ??= UpdateCommandHandler.Execute;
      this.optimizeAsync ??= OptimizeCommandHandler.Execute;

      var rootCmd = new Command("tinify", "Optimize media assets with Tinify.");

      rootCmd.AddCommand(Settings());
      rootCmd.AddCommand(Optimize());

      return rootCmd;

      Command Settings()
      {
        var settingsCmd = new Command("settings", "Manage Tinify settings.");

        settingsCmd.AddCommand(ListSettings());
        settingsCmd.AddCommand(UpdateSettings());

        return settingsCmd;
      }

      Command ListSettings()
      {
        var listCmd = new Command("list", "List Tinify settings.");

        // https://github.com/dotnet/command-line-api/issues/1537
        listCmd.SetHandler((context) =>
        {
          this.list(context.Console, context);
        });

        return listCmd;
      }

      Command UpdateSettings()
      {
        var option = new Option<string>("key", "Tinify API Key")
        {
          Arity = ArgumentArity.ExactlyOne,
        };

        var command = new Command("update", "Update Tinify settings.");
        command.Add(option);

        command.SetHandler((context) =>
        {
          string key = context.ParseResult.GetValueForOption(option);
          this.update(key, context.Console, context);
        });

        return command;
      }

      Command Optimize()
      {
        var option = new Option<FileInfo>("file-path", "Which image file (jpg|png) are you going to optimize?")
        {
          Arity = ArgumentArity.ExactlyOne,
        };

        var cmd = new Command("optimize", "Optimise images using Tinify")
        {
          option,
        };

        cmd.SetHandler(async (context) =>
        {
          FileInfo value = context.ParseResult.GetValueForOption(option);
          await this.optimizeAsync(value, context.Console, context).ConfigureAwait(false);
        });

        return cmd;
      }
    }
  }
}
