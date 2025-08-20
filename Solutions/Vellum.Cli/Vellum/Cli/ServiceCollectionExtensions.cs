// <copyright file="ServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

using Vellum.Abstractions.Content.Formatting;

namespace Vellum.Cli;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application wide services.
    /// </summary>
    /// <param name="serviceCollection">Application's ServiceCollection.</param>
    public static void AddCommonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMemoryCache();
        serviceCollection.AddTransient<ContentFormatter>();
    }
}