// <copyright file="ContentFragmentFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using Vellum.Abstractions.Caching;

    public static class ContentFragmentFactory
    {
        public static ContentFragment Create(ContentBlock contentBlock, string content)
        {
            return new ContentFragment
            {
                ContentType = contentBlock.ContentType,
                Hash = ContentHashing.Hash(content),
                Id = contentBlock.Id,
            };
        }
    }
}