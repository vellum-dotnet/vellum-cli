// <copyright file="ITaggable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public interface ITaggable
{
    IEnumerable<string> Tags { get; set; }
}