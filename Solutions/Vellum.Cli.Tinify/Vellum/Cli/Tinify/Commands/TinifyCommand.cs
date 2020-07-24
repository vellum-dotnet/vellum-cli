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

        public delegate Task Optimize(OptimizeOptions options, IConsole console, InvocationContext invocationContext = null);

        public delegate Task Update(UpdateOptions options, IConsole console, InvocationContext invocationContext = null);

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
                return new Command("list", "List Tinify settings.")
                {
                    Handler = CommandHandler.Create<InvocationContext>((context) =>
                    {
                        this.list(context.Console, context);
                    }),
                };
            }

            Command UpdateSettings()
            {
                var command = new Command("update", "Update Tinify settings.")
                {
                    new Argument<string>
                    {
                        Name = "Key",
                        Description = "Tinify API Key",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                command.Handler = CommandHandler.Create<UpdateOptions, InvocationContext>((options, context) =>
                {
                    this.update(options, context.Console, context);
                });

                return command;
            }

            Command Optimize()
            {
                var cmd = new Command("optimize", "Optimise images using Tinify")
                {
                    new Argument<FileInfo>
                    {
                        Name = "--file-path",
                        Description = "Which image file (jpg|png) are you going to optimize?",
                        Arity = ArgumentArity.ExactlyOne,
                    },
                };

                cmd.Handler = CommandHandler.Create<OptimizeOptions, InvocationContext>(async (options, context) =>
                {
                    await this.optimizeAsync(options, context.Console, context).ConfigureAwait(false);
                });

                return cmd;
            }
        }
    }
}
