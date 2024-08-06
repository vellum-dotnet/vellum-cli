// <copyright file="IExtensionTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.ContentFactories;

public interface IExtensionTypeFactory
{
    object Create(ContentFragment cf);
}