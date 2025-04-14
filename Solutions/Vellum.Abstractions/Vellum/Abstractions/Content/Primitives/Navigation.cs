// <copyright file="Navigation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Reflection.PortableExecutable;

namespace Vellum.Abstractions.Content.Primitives;

public record Navigation()
{
    public NavigationOption? Header { get; set; }

    public NavigationOption? Footer { get; set; }

    public Url? Parent { get; set; }

    public int Rank { get; set; }

    public Url? Url { get; set; }
}