// <copyright file="CloudinaryCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Cloudinary.Commands
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions.Commands;
    using Vellum.Cli.Cloudinary.Commands.Settings;
    using Vellum.Cli.Cloudinary.Commands.Upload;

    public class CloudinaryCommand : ICommandPlugin
    {
        private List list;
        private Update update;
        private Upload uploadAsync;

        public CloudinaryCommand()
        {
        }

        public CloudinaryCommand(List list, Update update, Upload uploadAsync)
        {
            this.list = list;
            this.update = update;
            this.uploadAsync = uploadAsync;
        }

        public delegate Task List(IConsole console, InvocationContext invocationContext = null);

        public delegate Task Upload(UploadOptions options, IConsole console, InvocationContext invocationContext = null);

        public delegate Task Update(UpdateOptions options, IConsole console, InvocationContext invocationContext = null);

        public Command Command()
        {
            this.list ??= ListCommandHandler.Execute;
            this.update ??= UpdateCommandHandler.Execute;
            this.uploadAsync ??= UploadCommandHandler.Execute;

            var rootCmd = new Command("cloudinary", "Manage media assets in Cloudinary.");

            rootCmd.AddCommand(Settings());
            rootCmd.AddCommand(Upload());

            return rootCmd;

            Command Settings()
            {
                var settingsCmd = new Command("settings", "Manage Cloudinary settings.");

                settingsCmd.AddCommand(ListSettings());
                settingsCmd.AddCommand(UpdateSettings());

                return settingsCmd;
            }

            Command ListSettings()
            {
                return new Command("list", "List Cloudinary settings.")
                {
                    Handler = CommandHandler.Create<InvocationContext>((context) =>
                    {
                        this.list(context.Console, context);
                    }),
                };
            }

            Command UpdateSettings()
            {
                var command = new Command("update", "Update Cloudinary settings.")
                {
                    new Option("--cloud", "Cloudinary Cloud Account Name")
                    {
                        Argument = new Argument<string>(),
                    },
                    new Option("--key", "Cloudinary API Key")
                    {
                        Argument = new Argument<string>(),
                    },
                    new Option("--secret", "Cloudinary API Secret")
                    {
                        Argument = new Argument<string>(),
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
                var cmd = new Command("upload", "Upload media assets in Cloudinary")
                {
                    new Option("--file-path", "Where should the demo project be written to?")
                    {
                        Argument = new Argument<FileInfo>(),
                    },
                };

                cmd.Handler = CommandHandler.Create<UploadOptions, InvocationContext>(async (options, context) =>
                {
                    await this.uploadAsync(options, context.Console, context).ConfigureAwait(false);
                });

                return cmd;
            }
        }
    }
}
