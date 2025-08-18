// <copyright file="IRenderingApi.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace Vellum.Abstractions.Rendering;

public interface IRenderingApi
{
    Task<string> RenderContent<TModel>(TModel content, string name);
}