// <copyright file="IConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions;

public interface IConverter<in T>
{
    object Convert(T value);
}