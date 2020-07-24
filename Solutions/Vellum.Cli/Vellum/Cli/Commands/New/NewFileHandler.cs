// <copyright file="NewFileHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.New
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using NDepend.Path;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Conventions;
    using Vellum.Cli.Abstractions.Environment;

    public static class NewFileHandler
    {
        public static async Task<int> ExecuteAsync(
            NewFileOptions fileOptions,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var settingsManager = new EnvironmentSettingsManager(appEnvironment);
            EnvironmentSettings environmentSettings = settingsManager.LoadSettings();

            if (environmentSettings.WorkspacePath != null && fileOptions.FilePath != null)
            {
                console.Error.WriteLine("You must either set a workspace via the environment command or supply a filepath.");
                return ReturnCodes.Error;
            }

            if (fileOptions.FilePath != null)
            {
                environmentSettings.WorkspacePath = fileOptions.FilePath.FullName;
            }

            List<ContentTypeConventionsRoot> conventions = await FindAllConventions(appEnvironment);

            // Select the convention that matches the template name specified
            ContentTypeConventionsRoot contentTypeConventionsRoot = conventions.FirstOrDefault(x => x.Conventions.Any(y => y.Conventions.Any(z => z.Value == fileOptions.TemplateName)));

            // Now find the filepath..
            Convention convention = contentTypeConventionsRoot?.Conventions.SelectMany(x => x.Conventions).FirstOrDefault(x => x.ContentType.StartsWith(ConventionContentTypes.FilePaths, StringComparison.CurrentCultureIgnoreCase));

            if (convention != null)
            {
                IVariableDirectoryPath variablePath = convention.Value.ToVariableDirectoryPath();
                IAbsoluteDirectoryPath evaluatedPath;

                if (variablePath.TryResolve(environmentSettings.ToKvPs(), out evaluatedPath) == VariablePathResolvingStatus.Success)
                {
                    var filepath = evaluatedPath.GetChildFileWithName($"post-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.md");

                    if (!filepath.ParentDirectoryPath.Exists)
                    {
                        Directory.CreateDirectory(filepath.ParentDirectoryPath.ToString());
                    }

                    string template = Path.Join(contentTypeConventionsRoot.FilePath.ParentDirectoryPath.ParentDirectoryPath.ToString(), contentTypeConventionsRoot?.Conventions.FirstOrDefault()?.TemplatePath);

                    File.Copy(template, filepath.ToString());
                }
            }

            return ReturnCodes.Ok;
        }

        private static async Task<List<ContentTypeConventionsRoot>> FindAllConventions(IAppEnvironment appEnvironment)
        {
            var conventionsManager = new ConventionsManager();
            var conventions = new List<ContentTypeConventionsRoot>();

            List<IAbsoluteFilePath> conventionFilePaths = FindAllConventionFiles(appEnvironment);

            foreach (IAbsoluteFilePath filePath in conventionFilePaths)
            {
                var convention = await conventionsManager.LoadAsync(filePath).ConfigureAwait(false);
                convention.FilePath = filePath;
                conventions.Add(convention);
            }

            return conventions;
        }

        private static List<IAbsoluteFilePath> FindAllConventionFiles(IAppEnvironment appEnvironment)
        {
            var conventionFilePaths = new List<IAbsoluteFilePath>();
            foreach (var pluginPath in appEnvironment.TemplatesPath.ChildrenDirectoriesPath)
            {
                foreach (var child in pluginPath.ChildrenDirectoriesPath)
                {
                    var current = child;

                    while (current != null)
                    {
                        if (current.GetChildFileWithName("conventions.json").Exists)
                        {
                            conventionFilePaths.Add(current.GetChildFileWithName("conventions.json"));
                        }

                        current = current.ChildrenDirectoriesPath.FirstOrDefault();
                    }
                }
            }

            return conventionFilePaths;
        }
    }
}
