// <copyright file="ContentExtensibilityOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

/// <summary>
/// Options controlling how content fragments are duck-typed onto their target interfaces.
/// </summary>
public class ContentExtensibilityOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether reading a member that exists neither in the fragment's
    /// metadata, nor on <see cref="ContentFragment"/>, nor on any target interface fails the dynamic
    /// binding — surfacing template typos as binder errors instead of silently returning null.
    /// </summary>
    public bool StrictMemberAccess { get; set; }
}