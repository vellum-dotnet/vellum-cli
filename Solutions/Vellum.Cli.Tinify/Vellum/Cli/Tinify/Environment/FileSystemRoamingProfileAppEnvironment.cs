// <copyright file="FileSystemRoamingProfileAppEnvironment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.IO;
using Vellum.Cli.Abstractions.Environment;

namespace Vellum.Cli.Tinify.Environment;

public class FileSystemRoamingProfileAppEnvironment : IAppEnvironmentConfiguration
{
    public const string AppName = "vellum-cli";
    public const string AppOrgName = "endjin";
    public const string ConfigurationDirectorName = "configuration";

    public DirectoryPath AppPath
    {
        get
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), AppOrgName, AppName);
        }
    }

    public DirectoryPath ConfigurationPath
    {
        get { return System.IO.Path.Combine(this.AppPath.ToString(), ConfigurationDirectorName); }
    }
}