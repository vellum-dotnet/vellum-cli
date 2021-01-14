// <copyright file="ContentTypeExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.ContentFactories
{
    public static class ContentTypeExtensions
    {
        public static string AsRenderer(this string contentType)
        {
            return contentType + "+renderer";
        }

        public static string AsMapper(this string contentType)
        {
            return contentType + "+mapper";
        }

        public static string AsContentFragmentFactory(this string contentType)
        {
            return contentType + "+content-fragment-factory";
        }
    }
}