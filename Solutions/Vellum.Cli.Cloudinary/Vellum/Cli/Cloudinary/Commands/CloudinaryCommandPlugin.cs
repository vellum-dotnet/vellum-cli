// <copyright file="CloudinaryCommandPlugin.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.Console.Cli;
using Vellum.Cli.Abstractions.Commands;
using Vellum.Cli.Cloudinary.Commands.Settings;
using Vellum.Cli.Cloudinary.Commands.Upload;

namespace Vellum.Cli.Cloudinary.Commands;

public class CloudinaryCommandPlugin : ICommandPlugin
{
    public void Configure(IConfigurator configurator)
    {
        configurator.AddBranch("cloudinary", root =>
        {
            root.SetDescription("Manage media assets in Cloudinary.");

            root.AddBranch("settings", settings =>
            {
                settings.SetDescription("Manage Cloudinary settings.");
                settings.AddCommand<ListCommand>("list")
                    .WithDescription("List Cloudinary settings.");
                settings.AddCommand<UpdateCommand>("update")
                    .WithDescription("Update Cloudinary settings.");
            });
            root.AddCommand<UploadCommand>("upload")
                .WithDescription("Upload media assets to Cloudinary.");
        });
    }
}

/*public class CloudinaryCommand
{
    public Command Command()
    {
        this.list ??= ListCommand.Execute;
        this.update ??= UpdateCommand.Execute;
        this.upload ??= UploadCommand.Execute;

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
}*/