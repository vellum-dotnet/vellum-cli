// <copyright file="ContentBlock.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

public record ContentBlock
{
    public ContentSpecification? Spec { get; init; }

    public required string ContentType { get; init; }

    public required string Id { get; init; }
}