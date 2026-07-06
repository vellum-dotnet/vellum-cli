// <copyright file="IConverter{TIn,TOut}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Converters;

/// <summary>
/// A strongly typed <see cref="IConverter{T}"/>. Implement this rather than the untyped interface so the
/// converted shape is visible in the type system; the untyped <see cref="IConverter{T}.Convert(T)"/> used
/// by the coercion pipeline is provided automatically.
/// </summary>
/// <typeparam name="TIn">The raw value type produced by the frontmatter parser.</typeparam>
/// <typeparam name="TOut">The converted value type declared by the target interface.</typeparam>
public interface IConverter<in TIn, TOut> : IConverter<TIn>
{
    /// <summary>
    /// Converts a raw frontmatter value into the shape declared by the target interface.
    /// </summary>
    /// <param name="value">The raw frontmatter value.</param>
    /// <returns>The converted value.</returns>
    new TOut Convert(TIn value);

    /// <inheritdoc/>
    object IConverter<TIn>.Convert(TIn value) => this.Convert(value)!;
}