// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli
{
    using System;
    using System.CommandLine.Parsing;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Vellum.Cli.Abstractions.Infrastructure;
    using Vellum.Cli.Environment;
    using Vellum.Cli.Plugins;

    public static class Program
    {
        private static readonly ServiceCollection ServiceCollection = new();

        public static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            ServiceCollection.AddCommonServices();

            return await new CommandLineParser(
                new CompositeConsole(),
                new FileSystemRoamingProfileAppEnvironment(),
                new CommandPluginHost(),
                ServiceCollection).Create().InvokeAsync(args).ConfigureAwait(false);
        }
    }
}