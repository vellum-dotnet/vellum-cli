// <copyright file="ContentFragmentTransformationPipeline.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Vellum.Abstractions.Content.Transformations;

/// <summary>
/// Orchestrates the transformation of content fragments through a pipeline of transformers.
/// </summary>
public class ContentFragmentTransformationPipeline
{
    private readonly IEnumerable<IContentFragmentTransformer> transformers;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentFragmentTransformationPipeline"/> class.
    /// </summary>
    /// <param name="transformers">The collection of transformers to apply.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    public ContentFragmentTransformationPipeline(
        IEnumerable<IContentFragmentTransformer> transformers,
        IServiceProvider serviceProvider)
    {
        this.transformers = transformers;
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Processes a content fragment through all applicable transformers in the pipeline.
    /// </summary>
    /// <param name="fragment">The content fragment to process.</param>
    /// <returns>The transformed content fragment.</returns>
    public ContentFragment Process(ContentFragment fragment)
    {
        ContentFragment result = fragment;

        foreach (IContentFragmentTransformer transformer in this.transformers)
        {
            if (transformer.CanTransform(result))
            {
                result = transformer.Transform(result, this.serviceProvider);
            }
        }

        return result;
    }
}