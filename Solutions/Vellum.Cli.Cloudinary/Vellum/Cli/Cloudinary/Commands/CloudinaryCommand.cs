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
            var settingsCmd = new Command("settings", "Manage Cloudinary settings.");

            settingsCmd.AddCommand(ListSettings());
            settingsCmd.AddCommand(UpdateSettings());

            return settingsCmd;

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
                var cloudOption = new Option<string>("--cloud")
                {
                    Description = "Cloudinary Cloud Account PackageId",
                    Arity = ArgumentArity.ExactlyOne,
                    IsRequired = true,
                };

                var keyOption = new Option<string>("--key")
                {
                    Description = "Cloudinary API Key",
                    Arity = ArgumentArity.ExactlyOne,
                    IsRequired = true,
                };

                var secretOption = new Option<string>("--secret")
                {
                    Description = "Cloudinary API Secret",
                    Arity = ArgumentArity.ExactlyOne,
                    IsRequired = true,
                };

                var cmd = new Command("update", "Update Cloudinary settings.")
                {
                    cloudOption, keyOption, secretOption,
                };

                cmd.SetHandler(context =>
                {
                    string cloud = context.ParseResult.GetValueForOption(cloudOption);
                    string key = context.ParseResult.GetValueForOption(keyOption);
                    string secret = context.ParseResult.GetValueForOption(secretOption);

                    this.update(cloud, key, secret, context.Console, context);
                });

                return cmd;
            }
        }

        Command Upload()
        {
            var option = new Option<FileInfo>("--file-path")
            {
                Arity = ArgumentArity.ExactlyOne,
                Description = "Which file should be uploaded to Cloudinary?",
                IsRequired = true,
            };

            var cmd = new Command("upload", "Upload media assets in Cloudinary")
            {
                option,
            };

            cmd.SetHandler(async context =>
            {
                FileInfo file = context.ParseResult.GetValueForOption(option);
                await this.upload(file, context.Console, context).ConfigureAwait(false);
            });

            return cmd;
        }
    }
}
