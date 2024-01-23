// <copyright file="PluginsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Environment;

namespace Vellum.Cli.Plugins;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class PluginsServiceCollectionExtensions
{
    /// <summary>
    /// Configures the service collection with the required dependencies for the command line application.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add to.</param>
    public static void ConfigurePluginDependencies(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ICommandPluginHost, CommandPluginHost>();
        serviceCollection.AddTransient<IAppEnvironment, FileSystemRoamingProfileAppEnvironment>();
    }
}