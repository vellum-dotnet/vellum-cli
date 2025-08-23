// <copyright file="InMemoryContentFragmentRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Vellum.Abstractions.Content;
using Vellum.Abstractions.Taxonomy;
using Vellum.Middleware.Abstractions;

namespace Vellum.Middleware.Repositories;

/// <summary>
/// In-memory implementation of IContentFragmentRepository that stores content fragments from the current context.
/// </summary>
public class InMemoryContentFragmentRepository : IContentFragmentRepository
{
    private readonly HashSet<ContentFragment> fragments = new();

    /// <summary>
    /// Initializes the repository with content fragments from the VellumContext.
    /// </summary>
    /// <param name="context">The VellumContext containing taxonomy documents with content fragments.</param>
    public void Initialize(VellumContext context)
    {
        this.fragments.Clear();

        foreach (TaxonomyDocument document in context.TaxonomyDocuments)
        {
            foreach (ContentFragment fragment in document.ContentFragments)
            {
                this.fragments.Add(fragment);
            }
        }
    }

    /// <inheritdoc />
    public ContentFragment? FindByMetaDataKey(string key, string contentType)
    {
        // Find fragment where the key matches either:
        // 1. The fragment's ID
        // 2. Any metadata value (for flexible matching)
        return this.fragments.FirstOrDefault(f =>
            f.ContentType == contentType &&
            (f.Id == key || // Match against fragment ID
             f.MetaData.Any(kvp => kvp.Value?.ToString() == key))); // Match against any metadata value
    }

    /// <inheritdoc />
    public IEnumerable<ContentFragment> GetAll() => this.fragments;

    /// <inheritdoc />
    public IEnumerable<ContentFragment> GetByContentType(string contentType) => this.fragments.Where(f => f.ContentType == contentType);
}