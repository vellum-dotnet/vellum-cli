// <copyright file="ContentFragmentTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using ImpromptuInterface;

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
        DynamicContentFragment cf = new(contentFragment, this.serviceProvider);

        return cf.ActLike<T>();
    }
}