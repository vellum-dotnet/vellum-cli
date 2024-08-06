// <copyright file="ITaggable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
namespace Vellum.Abstractions.Content;

using System.Collections.Generic;

public interface ITaggable
{
    IEnumerable<string> Tags { get; set; }
}