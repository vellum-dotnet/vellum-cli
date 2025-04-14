// <copyright file="Navigation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Reflection.PortableExecutable;

namespace Vellum.Abstractions.Content.Primitives;

public record Navigation(Url Parent, int Rank, Url Url)
{
    public NavigationOption? Header { get; set; }

    public NavigationOption? Footer { get; set; }
}