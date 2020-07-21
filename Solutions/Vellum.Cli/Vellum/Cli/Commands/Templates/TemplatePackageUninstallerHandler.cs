// <copyright file="TemplatePackageUninstallerHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Templates
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;

    public static class TemplatePackageUninstallerHandler
    {
        public static Task<int> ExecuteAsync(
            TemplateOptions options,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}