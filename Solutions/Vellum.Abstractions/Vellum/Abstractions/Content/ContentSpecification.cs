// <copyright file="ContentSpecification.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public record ContentSpecification(string? ContentType, int? Count, string? Path, List<string>? Tags);