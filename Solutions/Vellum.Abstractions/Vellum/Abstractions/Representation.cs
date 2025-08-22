// <copyright file="Representation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions;

public record Representation
{
    public required string ContentType { get; set; }

    public string GetContentType() => this.ContentType;
}