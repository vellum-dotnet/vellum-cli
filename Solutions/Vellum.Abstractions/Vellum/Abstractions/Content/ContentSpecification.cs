// <copyright file="ContentSpecification.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public record ContentSpecification
{
    public string? ContentType { get; init; }

    public int? Count { get; init; }

    public string? Path { get; init; }

    public List<string>? Tags { get; init; }
}