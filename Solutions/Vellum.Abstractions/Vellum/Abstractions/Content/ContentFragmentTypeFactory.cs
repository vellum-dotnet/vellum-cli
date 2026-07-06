// <copyright file="ContentFragmentTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;
using Microsoft.Extensions.DependencyInjection;

namespace Vellum.Abstractions.Content;

public class ContentFragmentTypeFactory<T>
    where T : class, IContent
{
    private readonly IServiceProvider serviceProvider;

    public ContentFragmentTypeFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public T Create(ContentFragment contentFragment)
    {
        Type[] targetTypes = this.ResolveTargetTypes(contentFragment);
        DynamicContentFragment cf = new(contentFragment, this.serviceProvider, targetTypes);

        return targetTypes.Length > 1 ? cf.ActLike<T>(targetTypes[1..]) : cf.ActLike<T>();
    }

    private Type[] ResolveTargetTypes(ContentFragment contentFragment)
    {
        List<Type> types = [typeof(T)];

        if (contentFragment.Extensions.Any())
        {
            // Lenient by design: without a registered IContentTypeInterfaceFactory, or for unknown
            // extension content types, the fragment is still projected onto T. Strict resolution
            // (throwing on unknown extensions) lives in ExtensionTypeFactory.
            IContentTypeInterfaceFactory? interfaceFactory = this.serviceProvider.GetService<IContentTypeInterfaceFactory>();

            if (interfaceFactory is not null)
            {
                foreach (string extension in contentFragment.Extensions)
                {
                    Type? extensionType = interfaceFactory.Resolve(extension);

                    if (extensionType is not null && !types.Contains(extensionType))
                    {
                        types.Add(extensionType);
                    }
                }
            }
        }

        return [.. types];
    }
}