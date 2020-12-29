// <copyright file="ContentListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;

    public static class ContentListHandler
    {
        public static Task<int> ExecuteAsync(
            ListOptions options,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            return Task.FromResult(ReturnCodes.Ok);
        }
    }
}
