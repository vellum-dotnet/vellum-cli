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