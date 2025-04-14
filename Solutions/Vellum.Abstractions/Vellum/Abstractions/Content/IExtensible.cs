// <copyright file="IExtensible.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public interface IExtensible
{
    public IEnumerable<string> Extensions { get; set; }
}