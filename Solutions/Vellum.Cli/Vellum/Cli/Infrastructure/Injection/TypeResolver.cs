// <copyright file="TypeResolver.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using Spectre.Console.Cli;

namespace Vellum.Cli.Infrastructure.Injection;

/// <summary>
/// Implementation of <see cref="ITypeResolver"/> that uses the <see cref="IServiceProvider"/> to resolve types.
/// </summary>
public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeResolver"/> class.
    /// </summary>
    /// <param name="provider">ServiceProvider for resolving types.</param>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="IServiceProvider"/> is null.</exception>
    public TypeResolver(IServiceProvider provider)
    {
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <summary>
    /// Resolves an instance of the type.
    /// </summary>
    /// <param name="type">Type to resolve.</param>
    /// <returns>Instance of the Type.</returns>
    public object? Resolve(Type? type)
    {
        return type == null ? null : this.provider.GetService(type);
    }

    /// <summary>
    /// Dispose the resolver.
    /// </summary>
    public void Dispose()
    {
        if (this.provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}