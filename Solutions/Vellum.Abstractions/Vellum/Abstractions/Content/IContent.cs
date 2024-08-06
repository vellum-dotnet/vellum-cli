// <copyright file="IContent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System;

    public interface IContent
    {
        string ContentType { get; set; }

        DateTime Date { get; set; }

        PublicationStatus PublicationStatus { get; set; }
    }
}