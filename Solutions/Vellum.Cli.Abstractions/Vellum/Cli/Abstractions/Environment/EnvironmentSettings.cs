// <copyright file="EnvironmentSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Environment
{
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentSettings
    {
        public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();

        public string Username { get; set; }

        public string PublishPath { get; set; }

        public string WorkspacePath { get; set; }

        public IEnumerable<KeyValuePair<string, string>> ToKvPs()
        {
            var dictionary = new Dictionary<string, string>
            {
                { nameof(this.Username).ToLowerInvariant(), this.Username },
                { nameof(this.PublishPath).ToLowerInvariant(), this.PublishPath },
                { nameof(this.WorkspacePath).ToLowerInvariant(), this.WorkspacePath },
            };

            return dictionary.Union(this.Configuration);
        }
    }
}