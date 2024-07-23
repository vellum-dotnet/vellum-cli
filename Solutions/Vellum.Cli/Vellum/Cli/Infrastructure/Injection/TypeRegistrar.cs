// <copyright file="TypeRegistrar.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Vellum.Cli.Infrastructure.Injection;

/// <summary>
/// Creates a type resolver from a service collection.
/// </summary>
public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeRegistrar"/> class.
    /// </summary>
    /// <param name="builder">ServiceCollection.</param>
    public TypeRegistrar(IServiceCollection builder)
    {
        this.builder = builder;
    }

    /// <summary>
    /// Builds the type resolver.
    /// </summary>
    /// <returns>A new TypeResolver.</returns>
    public ITypeResolver Build()
    {
        return new TypeResolver(this.builder.BuildServiceProvider());
    }

    /// <summary>
    /// Registers a type.
    /// </summary>
    /// <param name="service">Service Contract.</param>
    /// <param name="implementation">Implementation of the type.</param>
    public void Register(Type service, Type implementation)
    {
        this.builder.AddSingleton(service, implementation);
    }

    /// <summary>
    /// Registers an instance.
    /// </summary>
    /// <param name="service">Service Contract.</param>
    /// <param name="implementation">Implementation of the type.</param>
    public void RegisterInstance(Type service, object implementation)
    {
        this.builder.AddSingleton(service, implementation);
    }

    /// <summary>
    /// Registers a lazy type.
    /// </summary>
    /// <param name="service">Service Contract.</param>
    /// <param name="func">Function for providing the concrete type.</param>
    /// <exception cref="ArgumentNullException">Throws is hte func is null.</exception>
    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        this.builder.AddSingleton(service, (provider) => func());
    }
}