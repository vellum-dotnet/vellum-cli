﻿// <copyright file="NewFileHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1163,IDE0060 // Unused parameter - these methods are required to match certain signatures

namespace Vellum.Cli.Commands.New
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using NDepend.Path;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Conventions;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Infrastructure;

    public static class NewFileHandler
    {
        public static async Task<int> ExecuteAsync(
            string templateName,
            FileInfo filePath,
            ICompositeConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            var settingsManager = new EnvironmentSettingsManager(appEnvironment);
            EnvironmentSettings environmentSettings = settingsManager.LoadSettings();

            if (environmentSettings == null || (environmentSettings?.WorkspacePath != null && filePath != null))
            {
                console.Error.WriteLine("You must either set a workspace via the environment command or supply a filepath.");
                return ReturnCodes.Error;
            }

            if (filePath != null)
            {
                environmentSettings.WorkspacePath = filePath.FullName;
            }

            List<ContentTypeConventionsRoot> conventions = await FindAllConventions(appEnvironment);

            // Select the convention that matches the template name specified
            ContentTypeConventionsRoot contentTypeConventionsRoot = conventions.Find(x => x.Conventions.Any(y => y.Conventions.Any(z => z.Value == templateName)));

            // Now find the filepath..
            Convention convention = contentTypeConventionsRoot?.Conventions.SelectMany(x => x.Conventions)
                .FirstOrDefault(x => x.ContentType.StartsWith(ConventionContentTypes.FilePaths, StringComparison.CurrentCultureIgnoreCase));

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

                    console.Out.WriteLine($"Created: {filepath}");
                }
            }

            return ReturnCodes.Ok;
        }

        private static async Task<List<ContentTypeConventionsRoot>> FindAllConventions(IAppEnvironment appEnvironment)
        {
            var conventions = new List<ContentTypeConventionsRoot>();

            IEnumerable<IAbsoluteFilePath> conventionFilePaths = FindAllConventionFiles(appEnvironment);

            foreach (IAbsoluteFilePath filePath in conventionFilePaths)
            {
                ContentTypeConventionsRoot convention = await ConventionsManager.LoadAsync(filePath).ConfigureAwait(false);
                convention.FilePath = filePath;
                conventions.Add(convention);
            }

            return conventions;
        }

        private static IEnumerable<IAbsoluteFilePath> FindAllConventionFiles(IAppEnvironment appEnvironment)
        {
            var conventionFilePaths = new List<IAbsoluteFilePath>();

            foreach (IAbsoluteDirectoryPath pluginPath in appEnvironment.TemplatesPath.ChildrenDirectoriesPath)
            {
                foreach (IAbsoluteDirectoryPath child in pluginPath.ChildrenDirectoriesPath)
                {
                    IAbsoluteDirectoryPath current = child;

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