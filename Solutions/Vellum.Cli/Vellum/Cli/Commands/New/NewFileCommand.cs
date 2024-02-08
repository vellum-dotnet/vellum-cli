// <copyright file="NewFileCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Conventions;
using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Abstractions.Extensions;

namespace Vellum.Cli.Commands.New;

public class NewFileCommand(IAppEnvironment appEnvironment) : AsyncCommand<NewFileCommand.Settings>
{
    private readonly IAppEnvironment appEnvironment = appEnvironment;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        EnvironmentSettingsManager settingsManager = new(this.appEnvironment);
        EnvironmentSettings environmentSettings = settingsManager.LoadSettings(nameof(EnvironmentSettings));

        if (environmentSettings == null || (environmentSettings?.WorkspacePath != null && settings.FilePath != null))
        {
            AnsiConsole.WriteLine("You must either set a workspace via the environment command or supply a filepath.");
            return ReturnCodes.Error;
        }

        if (settings.FilePath != null && environmentSettings != null)
        {
            environmentSettings.WorkspacePath = settings.FilePath;
        }

        List<ContentTypeConventionsRoot> conventions = await this.FindAllConventions();

        // Select the convention that matches the template name specified
        ContentTypeConventionsRoot? contentTypeConventionsRoot = conventions.Find(x => x.Conventions.Any(y => y.Conventions.Any(z => z.Value == settings.TemplateName)));

        // Now find the filepath
        Convention? convention = contentTypeConventionsRoot?.Conventions.SelectMany(x => x.Conventions).FirstOrDefault(x => x.ContentType.StartsWith(ConventionContentTypes.FilePaths, StringComparison.CurrentCultureIgnoreCase));

        /* TODO
        if (convention != null)
        {
            IVariableDirectoryPath variablePath = convention.Value.ToVariableDirectoryPath();

            if (variablePath.TryResolve(environmentSettings.ToKvPs(), out IAbsoluteDirectoryPath evaluatedPath) == VariablePathResolvingStatus.Success)
            {
                IAbsoluteFilePath filepath = evaluatedPath.GetChildFileWithName($"post-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.md");

                if (!filepath.ParentDirectoryPath.Exists)
                {
                    Directory.CreateDirectory(filepath.ParentDirectoryPath.ToString());
                }

                string template = Path.Join(contentTypeConventionsRoot.FilePath.ParentDirectoryPath.ParentDirectoryPath.ToString(), contentTypeConventionsRoot?.Conventions.FirstOrDefault()?.TemplatePath);

                File.Copy(template, filepath.ToString());

                AnsiConsole.WriteLine($"Created: {filepath}");
            }
        }
        */

        return ReturnCodes.Ok;
    }

    private async Task<List<ContentTypeConventionsRoot>> FindAllConventions()
    {
        var conventions = new List<ContentTypeConventionsRoot>();

        IEnumerable<FilePath> conventionFilePaths = this.FindAllConventionFiles();

        foreach (FilePath filePath in conventionFilePaths)
        {
            ContentTypeConventionsRoot convention = await ConventionsManager.LoadAsync(filePath).ConfigureAwait(false);
            convention.FilePath = filePath;
            conventions.Add(convention);
        }

        return conventions;
    }

    private IEnumerable<FilePath> FindAllConventionFiles()
    {
        List<FilePath> conventionFilePaths = [];

        foreach (DirectoryPath pluginPath in this.appEnvironment.TemplatesPath.ChildrenDirectoriesPath())
        {
            foreach (DirectoryPath child in pluginPath.ChildrenDirectoriesPath())
            {
                DirectoryPath? current = child;

                while (current != null)
                {
                    if (System.IO.Path.Exists(current.GetChildFileWithName("conventions.json").FullPath))
                    {
                        conventionFilePaths.Add(current.GetChildFileWithName("conventions.json"));
                    }

                    current = current.ChildrenDirectoriesPath()[0];
                }
            }
        }

        return conventionFilePaths;
    }

    public class Settings : CommandSettings
    {
        [CommandOption("--template-name|-t")]
        [Description("Name of the template, as defined by the template convention")]
        public string? TemplateName { get; set; }

        [CommandOption("--file-path|-f")]
        [Description("Where do you want the new file to be created?")]
        public string? FilePath { get; set; }
    }
}