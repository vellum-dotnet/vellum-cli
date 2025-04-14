// <copyright file="ContentFragment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public record ContentFragment : IContent, IExtensible
{
    public int Position { get; set; }

    public required string Id { get; set; }

    public required string ContentType { get; set; }

    public IEnumerable<string> Extensions { get; set; } = [];

    public Dictionary<string, dynamic> MetaData { get; set; } = new();

    public required string Hash { get; set; }

    public required string Body { get; set; }

    public DateTime Date { get; set; }

    public PublicationStatus PublicationStatus { get; set; }
}