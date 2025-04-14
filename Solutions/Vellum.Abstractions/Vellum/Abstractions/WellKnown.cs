// <copyright file="WellKnown.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions;

public static class WellKnown
{
    public static class Taxonomies
    {
        public static class ContentTypes
        {
            public const string HomePage = "application/vnd.vellum.taxonomy.homepage+yaml";

            public const string Page = "application/vnd.vellum.taxonomy.page+yaml";

            public const string Site = "application/vnd.vellum.taxonomy.site+yaml";

            public static class Blog
            {
                public const string Index = "application/vnd.vellum.taxonomy.blog.index+yaml";

                public const string Post = "application/vnd.vellum.taxonomy.blog+yaml";

                public const string PostsByAuthor = "application/vnd.vellum.taxonomy.blog.by.author+yaml";

                public const string PostsByEdition = "application/vnd.vellum.taxonomy.blog.by.edition+yaml";
            }
        }
    }

    public static class ContentFragments
    {
        public static class ContentTypes
        {
            public const string Authors = "application/vnd.vellum.content.authors+md";

            public const string BlogMarkdown = "application/vnd.vellum.content.blogs+md";

            public const string ContentMarkdown = "application/vnd.vellum.content.content+md";

            public const string Hero = "application/vnd.vellum.content.hero+md";

            public const string ImageHero = "application/vnd.vellum.content.hero.with.image+md";
        }
    }
}