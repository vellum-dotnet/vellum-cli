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
        private Upload uploadAsync;

        public TinifyCommand()
        {
        }

        public TinifyCommand(List list, Update update, Upload uploadAsync)
        {
            this.list = list;
            this.update = update;
            this.uploadAsync = uploadAsync;
        }

        public delegate Task List(IConsole console, InvocationContext invocationContext = null);

        public delegate Task Upload(OptimizeOptions options, IConsole console, InvocationContext invocationContext = null);

        public delegate Task Update(UpdateOptions options, IConsole console, InvocationContext invocationContext = null);

        public Command Command()
        {
            this.list ??= ListCommandHandler.Execute;
            this.update ??= UpdateCommandHandler.Execute;
            this.uploadAsync ??= OptimizeCommandHandler.Execute;

            var rootCmd = new Command("tinify", "Optimize media assets with Tinify.");

            rootCmd.AddCommand(Settings());
            rootCmd.AddCommand(Upload());

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
                    new Option("--key", "Tinify API Key")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ExactlyOne,
                        },
                    },
                };

                command.Handler = CommandHandler.Create<UpdateOptions, InvocationContext>((options, context) =>
                {
                    this.update(options, context.Console, context);
                });

                return command;
            }

            Command Upload()
            {
                var cmd = new Command("optimize", "Optimise images using tinify")
                {
                    new Option("--file-path", "Which image file (jpg|png) are you going to optimize?")
                    {
                        Argument = new Argument<FileInfo>(),
                    },
                };

                cmd.Handler = CommandHandler.Create<OptimizeOptions, InvocationContext>(async (options, context) =>
                {
                    await this.uploadAsync(options, context.Console, context).ConfigureAwait(false);
                });

                return cmd;
            }
        }
    }
}
