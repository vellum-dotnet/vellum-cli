// <copyright file="IContentTransform.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Formatting
{
    public interface IContentTransform
    {
        string Apply(string input);
    }
}