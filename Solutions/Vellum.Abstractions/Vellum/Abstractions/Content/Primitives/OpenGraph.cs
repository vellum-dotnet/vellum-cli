// <copyright file="OpenGraph.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Primitives;

public record OpenGraph
{
    public OpenGraph()
    {
    }

    public OpenGraph(string title, string description, string image)
    {
        this.Title = title;
        this.Description = description;
        this.Image = image;
    }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Image { get; init; } = string.Empty;
}