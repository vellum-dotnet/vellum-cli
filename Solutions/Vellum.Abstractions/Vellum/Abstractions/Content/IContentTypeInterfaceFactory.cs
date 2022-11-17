// <copyright file="IContentTypeInterfaceFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

using System;

public interface IContentTypeInterfaceFactory
{
    Type Resolve(string contentType);
}