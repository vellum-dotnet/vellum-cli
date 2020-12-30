// <copyright file="PublicationStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    public enum PublicationStatus
    {
        /// <summary>
        /// Content is Published.
        /// </summary>
        Published,

        /// <summary>
        /// Content is in Draft.
        /// </summary>
        Draft,

        /// <summary>
        /// Content is in an unknown state.
        /// </summary>
        Unknown,
    }
}