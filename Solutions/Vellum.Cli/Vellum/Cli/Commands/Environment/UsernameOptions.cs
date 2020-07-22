// <copyright file="UsernameOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Environment
{
    using System.IO;

    public class UsernameOptions
    {
        public UsernameOptions(string username)
        {
            this.Username = username;
        }

        public string Username { get; }
    }
}