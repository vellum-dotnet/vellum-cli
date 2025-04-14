// <copyright file="PublicationStatusEnumParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content;

public static class PublicationStatusEnumParser
{
    public static PublicationStatus Parse(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "publish" => PublicationStatus.Published,
            "published" => PublicationStatus.Published,
            "draft" => PublicationStatus.Draft,
            _ => PublicationStatus.Unknown,
        };
    }
}