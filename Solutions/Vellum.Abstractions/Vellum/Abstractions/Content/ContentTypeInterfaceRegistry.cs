// <copyright file="ContentTypeInterfaceRegistry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

/// <summary>
/// Resolves content types to interfaces from the <see cref="ContentTypeInterfaceRegistration"/> entries
/// registered with the container, so hosts can map their own content types without modifying Vellum.
/// </summary>
public class ContentTypeInterfaceRegistry : IContentTypeInterfaceFactory
{
    private readonly Dictionary<string, Type> interfaces;

    public ContentTypeInterfaceRegistry(IEnumerable<ContentTypeInterfaceRegistration> registrations)
    {
        this.interfaces = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        foreach (ContentTypeInterfaceRegistration registration in registrations)
        {
            // Last registration wins, so hosts can override the well-known defaults.
            this.interfaces[registration.ContentType] = registration.InterfaceType;
        }
    }

    public Type? Resolve(string contentType)
    {
        return this.interfaces.TryGetValue(contentType, out Type? interfaceType) ? interfaceType : null;
    }
}