// <copyright file="Navigation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Primitives;

public class Navigation
{
    public Url Parent { get; set; }

    public int Rank { get; set; }

    public Url Url { get; set; }

#nullable enable

    public NavigationOption? Header { get; set; }

    public NavigationOption? Footer { get; set; }

#nullable disable
}