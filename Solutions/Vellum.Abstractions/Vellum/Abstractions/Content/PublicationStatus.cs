// <copyright file="PublicationStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

public enum PublicationStatus
{
    /// <summary>
    /// Content is published.
    /// </summary>
    Published,

    /// <summary>
    /// Content is in draft.
    /// </summary>
    Draft,

    /// <summary>
    /// Content is in an unknown state.
    /// </summary>
    Unknown,

    /// <summary>
    /// Content is unpublished.
    /// </summary>
    Unpublished,
}