// <copyright file="IContentTypeInterfaceFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Vellum.Abstractions.Content;

public interface IContentTypeInterfaceFactory
{
    Type Resolve(string contentType);
}