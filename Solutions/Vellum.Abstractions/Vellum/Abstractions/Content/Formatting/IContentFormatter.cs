// <copyright file="IContentFormatter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Formatting;

public interface IContentFormatter
{
    string Apply(string html);
}