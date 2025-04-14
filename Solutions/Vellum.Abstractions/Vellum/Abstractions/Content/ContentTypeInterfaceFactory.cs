// <copyright file="ContentTypeInterfaceFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using Vellum.Abstractions.Content.Extensions;

namespace Vellum.Abstractions.Content;

public class ContentTypeInterfaceFactory : IContentTypeInterfaceFactory
{
    public Type Resolve(string contentType)
    {
        switch (contentType)
        {
            case WellKnown.ContentFragments.ContentTypes.BlogMarkdown:
                return typeof(IBlogPost);
            case "application/vnd.vellum.content.series+md":
                return typeof(ISeries);
            case "application/vnd.vellum.content.promotion+md":
                return typeof(IPromotions);
            default:
                return null;
        }
    }
}