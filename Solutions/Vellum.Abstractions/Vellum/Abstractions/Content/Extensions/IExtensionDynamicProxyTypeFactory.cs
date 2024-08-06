// <copyright file="IExtensionDynamicProxyTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Extensions;

using System;
using System.Collections.Generic;

public interface IExtensionDynamicProxyTypeFactory
{
    Type Create(Type baseType, IEnumerable<Type> extensionTypes);
}