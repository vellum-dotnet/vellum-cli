// <copyright file="ContentListHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;

    using Vellum.Abstractions;
    using Vellum.Cli.Abstractions;
    using Vellum.Cli.Abstractions.Environment;

    public static class ContentListHandler
    {
        public static async Task<int> ExecuteAsync(
            ListOptions options,
            IConsole console,
            IAppEnvironment appEnvironment,
            InvocationContext context = null)
        {
            SiteTaxonomy siteTaxonomy = await new SiteTaxonomyRepository().FindAsync(options.SiteTaxonomyDirectoryPath).ConfigureAwait(false);

            return ReturnCodes.Ok;
        }
    }
}
