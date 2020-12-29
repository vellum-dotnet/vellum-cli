// <copyright file="ContentBlock.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    public class ContentBlock
    {
        public ContentSpecification Spec { get; set; }

        public string ContentType { get; set; }

        public string Id { get; set; }
    }
}
