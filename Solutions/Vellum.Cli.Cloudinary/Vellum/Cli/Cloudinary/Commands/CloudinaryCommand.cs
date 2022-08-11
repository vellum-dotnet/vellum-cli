// <copyright file="CloudinaryCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Cloudinary.Commands;

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
    private Upload upload;

    public CloudinaryCommand()
    {
    }

    public CloudinaryCommand(List list, Update update, Upload upload)
    {
        this.list = list;
        this.update = update;
        this.upload = upload;
    }

    public delegate Task List(IConsole console, InvocationContext invocationContext = null);

    public delegate Task Update(string cloud, string key, string secret, IConsole console, InvocationContext invocationContext = null);

    public delegate Task Upload(FileInfo file, IConsole console, InvocationContext invocationContext = null);

    public Command Command()
    {
        this.list ??= ListCommandHandler.Execute;
        this.update ??= UpdateCommandHandler.Execute;
        this.upload ??= UploadCommandHandler.Execute;

        var rootCmd = new Command("cloudinary", "Manage media assets in Cloudinary.");

        rootCmd.AddCommand(Settings());
        rootCmd.AddCommand(Upload());

        return rootCmd;

        Command Settings()
        {
            var cmd = new Command("settings", "Manage Cloudinary settings.");

            cmd.AddCommand(ListSettings());
            cmd.AddCommand(UpdateSettings());

            return cmd;

            Command ListSettings()
            {
                var cmd = new Command("list", "List Cloudinary settings.");

                cmd.SetHandler(context =>
                {
                    this.list(context.Console, context);
                });

                return cmd;
            }

            Command UpdateSettings()
            {
                var cloudArg = new Argument<string>
                {
                    Name = "cloud",
                    Description = "Cloudinary Cloud Account PackageId",
                    Arity = ArgumentArity.ExactlyOne,
                };

                var keyArg = new Argument<string>
                {
                    Name = "key",
                    Description = "Cloudinary API Key",
                    Arity = ArgumentArity.ExactlyOne,
                };

                var secretArg = new Argument<string>
                {
                    Name = "secret",
                    Description = "Cloudinary API Secret",
                    Arity = ArgumentArity.ExactlyOne,
                };

                var cmd = new Command("update", "Update Cloudinary settings.")
                {
                    cloudArg, keyArg, secretArg,
                };

                cmd.SetHandler(context =>
                {
                    string cloud = context.ParseResult.GetValueForArgument(cloudArg);
                    string key = context.ParseResult.GetValueForArgument(keyArg);
                    string secret = context.ParseResult.GetValueForArgument(secretArg);

                    this.update(cloud, key, secret, context.Console, context);
                });

                return cmd;
            }
        }

        Command Upload()
        {
            var argument = new Argument<FileInfo>("file-path", "Which file should be uploaded to Cloudinary?")
            {
                Arity = ArgumentArity.ExactlyOne,
            };

            var cmd = new Command("upload", "Upload media assets in Cloudinary")
            {
                argument,
            };

            cmd.SetHandler(async context =>
            {
                FileInfo file = context.ParseResult.GetValueForArgument(argument);
                await this.upload(file, context.Console, context).ConfigureAwait(false);
            });

            return cmd;
        }
    }
}