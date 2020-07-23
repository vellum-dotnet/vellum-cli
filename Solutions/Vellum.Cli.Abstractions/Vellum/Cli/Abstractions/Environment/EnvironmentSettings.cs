// <copyright file="EnvironmentSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Environment
{
    using System.Collections.Generic;

    public class EnvironmentSettings
    {
        public Dictionary<string, dynamic> Configuration { get; set; } = new Dictionary<string, dynamic>();

        public string Username { get; set; }

        public string PublishPath { get; set; }

        public string WorkspacePath { get; set; }
    }
}