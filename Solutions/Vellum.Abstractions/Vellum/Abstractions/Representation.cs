// <copyright file="Representation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions
{
    public class Representation
    {
        public string ContentType { get; set; }

        public string GetContentType()
        {
            return this.ContentType;
        }
    }
}