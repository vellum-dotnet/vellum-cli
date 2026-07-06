// <copyright file="ExtensionTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;

namespace Vellum.Abstractions.Content.ContentFactories;

public class ExtensionTypeFactory : IExtensionTypeFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly IContentTypeInterfaceFactory contentTypeInterfaceFactory;

    public ExtensionTypeFactory(IServiceProvider serviceProvider, IContentTypeInterfaceFactory contentTypeInterfaceFactory)
    {
        this.serviceProvider = serviceProvider;
        this.contentTypeInterfaceFactory = contentTypeInterfaceFactory;
    }

    public object? Create(ContentFragment cf)
    {
        if (!cf.Extensions.Any())
        {
            return null;
        }

        List<Type> targetTypes = [this.ResolveOrThrow(cf, cf.ContentType)];

        foreach (string extension in cf.Extensions)
        {
            Type extensionType = this.ResolveOrThrow(cf, extension);

            if (!targetTypes.Contains(extensionType))
            {
                targetTypes.Add(extensionType);
            }
        }

        Type[] types = [.. targetTypes];
        DynamicContentFragment dynamicFragment = new(cf, this.serviceProvider, types);

        return Impromptu.DynamicActLike(dynamicFragment, types);
    }

    private Type ResolveOrThrow(ContentFragment cf, string contentType)
    {
        Type? resolved = this.contentTypeInterfaceFactory.Resolve(contentType);

        if (resolved is null)
        {
            cf.MetaData.TryGetValue("FilePath", out dynamic? filePath);

            throw new InvalidOperationException(
                $"No content type interface is registered for content type '{contentType}' (content fragment '{cf.Id}'{(filePath is null ? string.Empty : $", file '{filePath}'")}). Register one via services.AddContentTypeInterface<TInterface>(\"{contentType}\").");
        }

        return resolved;
    }
}