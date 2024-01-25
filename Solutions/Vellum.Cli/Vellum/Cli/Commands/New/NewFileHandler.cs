// <copyright file="NewFileHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Spectre.Console;
using Spectre.IO;

using Vellum.Cli.Abstractions;
using Vellum.Cli.Abstractions.Conventions;
using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Abstractions.Extensions;

namespace Vellum.Cli.Commands.New;

public static class NewFileHandler
{
    public static async Task<int> ExecuteAsync(string templateName,  FileInfo filePath, IAppEnvironment appEnvironment)
    {
        var settingsManager = new EnvironmentSettingsManager(appEnvironment);
        EnvironmentSettings environmentSettings = settingsManager.LoadSettings(nameof(EnvironmentSettings));

        if (environmentSettings == null || (environmentSettings?.WorkspacePath != null && filePath != null))
        {
            AnsiConsole.WriteLine("You must either set a workspace via the environment command or supply a filepath.");
            return ReturnCodes.Error;
        }

        if (filePath != null && environmentSettings != null)
        {
            environmentSettings.WorkspacePath = filePath.FullName;
        }

        List<ContentTypeConventionsRoot> conventions = await FindAllConventions(appEnvironment);

        // Select the convention that matches the template name specified
        ContentTypeConventionsRoot? contentTypeConventionsRoot = conventions.Find(x => x.Conventions.Any(y => y.Conventions.Any(z => z.Value == templateName)));

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

    private static async Task<List<ContentTypeConventionsRoot>> FindAllConventions(IAppEnvironment appEnvironment)
    {
        var conventions = new List<ContentTypeConventionsRoot>();

        IEnumerable<FilePath> conventionFilePaths = FindAllConventionFiles(appEnvironment);

        foreach (FilePath filePath in conventionFilePaths)
        {
            ContentTypeConventionsRoot convention = await ConventionsManager.LoadAsync(filePath).ConfigureAwait(false);
            convention.FilePath = filePath;
            conventions.Add(convention);
        }

        return conventions;
    }

    private static IEnumerable<FilePath> FindAllConventionFiles(IAppEnvironment appEnvironment)
    {
        List<FilePath> conventionFilePaths = [];

        foreach (DirectoryPath pluginPath in appEnvironment.TemplatesPath.ChildrenDirectoriesPath())
        {
            foreach (DirectoryPath child in pluginPath.ChildrenDirectoriesPath())
            {
                DirectoryPath? current = child;

                while (current != null)
                {
                    /* TODO
                    if (current.GetChildFileWithName("conventions.json").Exists)
                    {
                        conventionFilePaths.Add(current.GetChildFileWithName("conventions.json"));
                    }*/

                    current = current.ChildrenDirectoriesPath()[0];
                }
            }
        }

        return conventionFilePaths;
    }
}