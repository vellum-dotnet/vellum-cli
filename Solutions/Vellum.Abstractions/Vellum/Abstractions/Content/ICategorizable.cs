// <copyright file="ICategorizable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public interface ICategorizable
{
    IEnumerable<string> Category { get; set; }
}