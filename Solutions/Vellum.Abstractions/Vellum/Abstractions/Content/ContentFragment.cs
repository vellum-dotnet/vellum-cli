// <copyright file="ContentFragment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System;
    using System.Collections.Generic;

    public record ContentFragment : IContent
    {
        public int Position { get; set; }

        public string Id { get; set; }

        public string ContentType { get; set; }

        public Dictionary<string, dynamic> MetaData { get; set; } = new();

        public string Hash { get; set; }

        public string Body { get; set; }

        public DateTime Date { get; set; }

        public PublicationStatus PublicationStatus { get; set; }
    }
}