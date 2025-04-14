// <copyright file="IExtensionDynamicProxyTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Vellum.Abstractions.Content.Extensions;

public interface IExtensionDynamicProxyTypeFactory
{
    Type? Create(Type baseType, IEnumerable<Type> extensionTypes);
}