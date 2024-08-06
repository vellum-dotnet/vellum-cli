// <copyright file="IExtensible.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

using System.Collections.Generic;

public interface IExtensible
{
    public IEnumerable<string> Extensions { get; set; }
}