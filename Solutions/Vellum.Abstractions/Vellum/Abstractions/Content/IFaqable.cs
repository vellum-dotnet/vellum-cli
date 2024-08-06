// <copyright file="IFaqable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

using System.Collections.Generic;

public interface IFaqable
{
    IEnumerable<(string Question, string Answer)> Faqs { get; set; }
}