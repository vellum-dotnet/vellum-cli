// <copyright file="ContentSpecification.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public class ContentSpecification
{
    public string ContentType { get; set; }

    public int Count { get; set; }

    public string Path { get; set; }

    public List<string> Tags { get; set; } = new List<string>();
}