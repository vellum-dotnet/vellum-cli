// <copyright file="IContentFragmentTransformer.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Vellum.Abstractions.Content.Transformations;

/// <summary>
/// Defines a transformer that can process and modify content fragments.
/// </summary>
public interface IContentFragmentTransformer
{
    /// <summary>
    /// Determines whether this transformer can process the given content fragment.
    /// </summary>
    /// <param name="fragment">The content fragment to check.</param>
    /// <returns>True if this transformer can process the fragment; otherwise, false.</returns>
    bool CanTransform(ContentFragment fragment);

    /// <summary>
    /// Transforms the given content fragment.
    /// </summary>
    /// <param name="fragment">The content fragment to transform.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <returns>The transformed content fragment.</returns>
    ContentFragment Transform(ContentFragment fragment, IServiceProvider serviceProvider);
}