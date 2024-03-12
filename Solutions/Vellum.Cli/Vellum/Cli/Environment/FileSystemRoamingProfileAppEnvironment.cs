// <copyright file="FileSystemRoamingProfileAppEnvironment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Spectre.Console;
using Spectre.IO;

using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Abstractions.Extensions;

namespace Vellum.Cli.Environment;

public class FileSystemRoamingProfileAppEnvironment : IAppEnvironment
{
    public const string AppName = "vellum-cli";
    public const string AppOrgName = "endjin";
    public const string ConfigurationDirectorName = "configuration";
    public const string PluginsDirectoryName = "plugins";
    public const string TemplatesDirectoryName = "templates";
    public const string NuGetFileName = "NuGet.Config";

    public const string DefaultNuGetConfig = """
                                             <?xml version="1.0" encoding="utf-8"?>
                                             <configuration>
                                                 <packageSources>
                                                     <add key="NuGet official package source" value="https://api.nuget.org/v3/index.json" />
                                                 </packageSources>
                                             </configuration>
                                             """;

    public DirectoryPath AppPath
    {
        get
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), AppOrgName, AppName);
        }
    }

    public DirectoryPath TemplatesPath
    {
        get
        {
            return System.IO.Path.Combine(this.AppPath.ToString()!, TemplatesDirectoryName);
        }
    }

    public DirectoryPath PluginPath
    {
        get
        {
            return System.IO.Path.Combine(this.AppPath.ToString()!, PluginsDirectoryName);
        }
    }

    public IEnumerable<DirectoryPath> PluginPaths
    {
        get
        {
            if (Debugger.IsAttached)
            {
                string? directory = AppContext.BaseDirectory;

                while (!Directory.Exists(System.IO.Path.Combine(directory!, ".git")))
                {
                    directory = Directory.GetParent(directory!)?.FullName;
                }

                IEnumerable<string> dirs = Directory.EnumerateDirectories(directory!, "*.*", SearchOption.AllDirectories);

                dirs = dirs.Where(f => !Directory.EnumerateDirectories(f, "*.*", SearchOption.TopDirectoryOnly).Any() && f.EndsWith(@"bin\Debug\net8.0"));

                foreach (string dir in dirs)
                {
                    yield return dir;
                }
            }
            else
            {
                foreach (DirectoryPath path in this.PluginPath.ChildrenDirectoriesPath())
                {
                    IEnumerable<DirectoryPath> leafPaths = Directory
                        .EnumerateDirectories(path.ToString(), "*.*", SearchOption.AllDirectories)
                        .Where(f => !Directory.EnumerateDirectories(f, "*.*", SearchOption.TopDirectoryOnly).Any())
                        .Select(x => new DirectoryPath(x));

                    foreach (DirectoryPath leafPath in leafPaths)
                    {
                        yield return leafPath;
                    }
                }
            }
        }
    }

    public DirectoryPath ConfigurationPath
    {
        get { return System.IO.Path.Combine(this.AppPath.ToString()!, ConfigurationDirectorName); }
    }

    public FilePath NuGetConfigFilePath
    {
        get { return System.IO.Path.Combine(this.ConfigurationPath.ToString()!, NuGetFileName); }
    }

    public void Clean()
    {
        Directory.Delete(this.AppPath.ToString()!, recursive: true);
    }

    public async Task InitializeAsync()
    {
        if (!Directory.Exists(this.AppPath.ToString()))
        {
            AnsiConsole.MarkupLine($"Creating {this.AppPath}");
            Directory.CreateDirectory(this.AppPath.ToString()!);
        }

        if (!Directory.Exists(this.ConfigurationPath.ToString()))
        {
            AnsiConsole.MarkupLine($"Creating {this.ConfigurationPath}");
            Directory.CreateDirectory(this.ConfigurationPath.ToString()!);
        }

        await using (StreamWriter writer = File.CreateText(this.NuGetConfigFilePath.ToString()!))
        {
            AnsiConsole.MarkupLine($"Creating {this.NuGetConfigFilePath}");
            await writer.WriteAsync(DefaultNuGetConfig).ConfigureAwait(false);
        }

        if (!Directory.Exists(this.PluginPath.ToString()))
        {
            AnsiConsole.MarkupLine($"Creating {this.PluginPath}");
            Directory.CreateDirectory(this.PluginPath.ToString()!);
        }

        if (!Directory.Exists(this.TemplatesPath.ToString()))
        {
            AnsiConsole.MarkupLine($"Creating {this.TemplatesPath}");
            Directory.CreateDirectory(this.TemplatesPath.ToString()!);
        }
    }

    public bool IsInitialized()
    {
        return Directory.Exists(this.AppPath.ToString()) &&
               Directory.Exists(this.TemplatesPath.ToString()) &&
               Directory.Exists(this.ConfigurationPath.ToString()) &&
               Directory.Exists(this.TemplatesPath.ToString()) &&
               Directory.Exists(this.PluginPath.ToString());
    }
}