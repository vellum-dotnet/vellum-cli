// <copyright file="ContentTypeInterfaceRegistration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Vellum.Abstractions.Content;

/// <summary>
/// Maps a content type (as declared in YAML frontmatter <c>ContentType</c> or <c>Extensions</c>) to the
/// .NET interface that content fragments of that type are duck-typed onto.
/// </summary>
public sealed record ContentTypeInterfaceRegistration(string ContentType, Type InterfaceType);