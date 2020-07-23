// <copyright file="SetOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.IO;

    public class SetOptions
    {
        public SetOptions(
            string username = null,
            DirectoryInfo workspacePath = null,
            DirectoryInfo publishPath = null,
            string key = null,
            string value = null)
        {
            this.Username = username;
            this.WorkspacePath = workspacePath;
            this.PublishPath = publishPath;
            this.Key = key;
            this.Value = value;
        }

        public DirectoryInfo WorkspacePath { get; }

        public DirectoryInfo PublishPath { get; }

        public string Key { get; }

        public string Value { get; }

        public string Username { get; }
    }
}