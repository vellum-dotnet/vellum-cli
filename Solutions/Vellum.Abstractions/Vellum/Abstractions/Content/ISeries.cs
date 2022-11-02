// <copyright file="ISeries.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

#pragma warning disable CS8632
public interface ISeries
{
    string PartTitle { get; set; }

    string Series { get; set; }
}