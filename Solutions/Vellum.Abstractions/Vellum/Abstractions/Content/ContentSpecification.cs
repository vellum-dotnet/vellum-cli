// <copyright file="ContentSpecification.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System.Collections.Generic;

    public class ContentSpecification
    {
        public string ContentType { get; set; }

        public int Count { get; set; }

        public string Path { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}