// <copyright file="ICategorizable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
namespace Vellum.Abstractions.Content;

using System.Collections.Generic;

public interface ICategorizable
{
    IEnumerable<string> Category { get; set; }
}