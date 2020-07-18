﻿// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli
{
    using System;
    using System.CommandLine.Parsing;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Vellum.Cli.Environment;
    using Vellum.Cli.Plugins;

    public static class Program
    {
        private static readonly ServiceCollection ServiceCollection = new ServiceCollection();

        public static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            return await new CommandLineParser(
                new FileSystemRoamingProfileAppEnvironment(),
                new CommandPluginHost(),
                ServiceCollection).Create().InvokeAsync(args).ConfigureAwait(false);
        }
    }
}