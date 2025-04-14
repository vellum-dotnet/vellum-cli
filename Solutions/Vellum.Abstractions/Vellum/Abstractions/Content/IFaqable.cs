// <copyright file="IFaqable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public interface IFaqable
{
    IEnumerable<(string Question, string Answer)> Faqs { get; set; }
}