// <copyright file="CoercionFailure.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Vellum.Abstractions.Content;

/// <summary>
/// Describes a metadata value that could not be coerced to the property type declared by a target interface.
/// </summary>
/// <param name="MemberName">The metadata member (frontmatter key or interface property) that failed to coerce.</param>
/// <param name="RawValue">The raw metadata value that failed to coerce.</param>
/// <param name="TargetType">The property type declared by the target interface.</param>
/// <param name="FilePath">The source file the content fragment was parsed from, when known.</param>
/// <param name="Reason">A human-readable description of why coercion failed.</param>
public sealed record CoercionFailure(string MemberName, object? RawValue, Type TargetType, string? FilePath, string Reason);