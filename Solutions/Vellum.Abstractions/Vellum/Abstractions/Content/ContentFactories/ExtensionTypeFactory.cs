// <copyright file="ExtensionTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Vellum.Abstractions.Content.Extensions;

namespace Vellum.Abstractions.Content.ContentFactories;

public class ExtensionTypeFactory : IExtensionTypeFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly IContentTypeInterfaceFactory contentTypeInterfaceFactory;
    private readonly IExtensionDynamicProxyTypeFactory extensionDynamicProxyTypeFactory;

    public ExtensionTypeFactory(IServiceProvider serviceProvider, IContentTypeInterfaceFactory contentTypeInterfaceFactory, IExtensionDynamicProxyTypeFactory extensionDynamicProxyTypeFactory)
    {
        this.serviceProvider = serviceProvider;
        this.contentTypeInterfaceFactory = contentTypeInterfaceFactory;
        this.extensionDynamicProxyTypeFactory = extensionDynamicProxyTypeFactory;
    }

    public object? Create(ContentFragment cf)
    {
        if (!cf.Extensions.Any())
        {
            return null;
        }

        IEnumerable<Type> extensionTypes = cf.Extensions.Select(contentType => this.contentTypeInterfaceFactory.Resolve(contentType)).Where(result => result != null);

        Type contentFragmentType = this.contentTypeInterfaceFactory.Resolve(cf.ContentType);
        Type extensionDynamicProxyType = this.extensionDynamicProxyTypeFactory.Create(contentFragmentType, extensionTypes);
        Type typeFactory = typeof(ContentFragmentTypeFactory<>);
        Type[] typeArgs = [extensionDynamicProxyType];
        Type genericTypeFactory = typeFactory.MakeGenericType(typeArgs);
        dynamic? typeFactoryInstance = Activator.CreateInstance(genericTypeFactory, args: this.serviceProvider);

        if (typeFactoryInstance == null)
        {
            throw new InvalidOperationException();
        }

        return typeFactoryInstance.Create(cf);
    }
}