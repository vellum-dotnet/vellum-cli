// <copyright file="FileSystemRoamingProfileAppEnvironment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Cloudinary.Environment
{
    using System;
    using System.IO;
    using NDepend.Path;
    using Vellum.Cli.Abstractions.Environment;

    public class FileSystemRoamingProfileAppEnvironment : IAppEnvironmentConfiguration
    {
        public const string AppName = "vellum-cli";
        public const string AppOrgName = "endjin";
        public const string ConfigurationDirectorName = "configuration";

        public IAbsoluteDirectoryPath AppPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppOrgName, AppName).ToAbsoluteDirectoryPath();
            }
        }

        public IAbsoluteDirectoryPath ConfigurationPath
        {
            get { return Path.Combine(this.AppPath.ToString(), ConfigurationDirectorName).ToAbsoluteDirectoryPath(); }
        }
    }
}