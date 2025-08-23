// <copyright file="AuthorIdResolver.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Vellum.Abstractions;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.Transformations;

namespace Vellum.Middleware.Transformations;

/// <summary>
/// Transformer that resolves AuthorId references to full Author content fragments.
/// </summary>
public class AuthorIdResolver : IContentFragmentTransformer
{
    private readonly ILogger<AuthorIdResolver>? logger;

    /// <summary>
    /// Initializes a new instance of the AuthorIdResolver class.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    public AuthorIdResolver(ILogger<AuthorIdResolver>? logger = null)
    {
        this.logger = logger;
    }

    /// <inheritdoc />
    public bool CanTransform(ContentFragment fragment)
    {
        return fragment.ContentType == WellKnown.ContentFragments.ContentTypes.BlogMarkdown
            && fragment.MetaData.ContainsKey("AuthorId");
    }

    /// <inheritdoc />
    public ContentFragment Transform(ContentFragment fragment, IServiceProvider serviceProvider)
    {
        if (!fragment.MetaData.TryGetValue("AuthorId", out dynamic? authorId))
        {
            return fragment;
        }

        string? authorIdString = authorId?.ToString();
        if (string.IsNullOrEmpty(authorIdString))
        {
            this.logger?.LogWarning("AuthorId is null or empty in blog post");
            return fragment;
        }

        IContentFragmentRepository? repository = serviceProvider.GetService<IContentFragmentRepository>();

        if (repository == null)
        {
            this.logger?.LogWarning("IContentFragmentRepository not found in service provider");
            return fragment;
        }

        ContentFragment? authorFragment = repository.FindByMetaDataKey(authorIdString, WellKnown.ContentFragments.ContentTypes.Authors);

        if (authorFragment == null)
        {
            this.logger?.LogWarning("Author not found: {AuthorId}", authorIdString);
            return fragment;
        }

        Dictionary<string, dynamic> updatedMetaData = new(fragment.MetaData);
        updatedMetaData.Remove("AuthorId");
        updatedMetaData["Author"] = authorFragment.MetaData;

        ContentFragment enrichedFragment = fragment with
        {
            MetaData = updatedMetaData
        };

        return enrichedFragment;
    }
}