// <copyright file="ContentBlock.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

public record ContentBlock(ContentSpecification Spec, string ContentType, string Id);