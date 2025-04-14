// <copyright file="IContent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Vellum.Abstractions.Content;

public interface IContent
{
    string ContentType { get; set; }

    DateTime Date { get; set; }

    PublicationStatus PublicationStatus { get; set; }
}