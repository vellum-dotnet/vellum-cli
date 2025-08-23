// <copyright file="IContentFragmentRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

/// <summary>
/// Provides access to content fragments across all taxonomy documents.
/// </summary>
public interface IContentFragmentRepository
{
    /// <summary>
    /// Finds a content fragment by matching a key against fragment ID or metadata values.
    /// </summary>
    /// <param name="key">The key to search for in fragment ID or metadata values.</param>
    /// <param name="contentType">The content type of the fragment.</param>
    /// <returns>The content fragment if found; otherwise, null.</returns>
    ContentFragment? FindByMetaDataKey(string key, string contentType);

    /// <summary>
    /// Gets all content fragments in the repository.
    /// </summary>
    /// <returns>An enumerable collection of all content fragments.</returns>
    IEnumerable<ContentFragment> GetAll();

    /// <summary>
    /// Gets all content fragments of a specific content type.
    /// </summary>
    /// <param name="contentType">The content type to filter by.</param>
    /// <returns>An enumerable collection of content fragments matching the content type.</returns>
    IEnumerable<ContentFragment> GetByContentType(string contentType);
}