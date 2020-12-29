// <copyright file="WellKnown.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions
{
    public static class WellKnown
    {
        public static class ContentTypes
        {
            public const string HomePage = "application/vnd.vellum.taxonomy.homepage+yaml";

            public const string Page = "application/vnd.vellum.taxonomy.page+yaml";

            public const string Site = "application/vnd.vellum.taxonomy.site+yml";

            public static class Blog
            {
                public const string Index = "application/vnd.vellum.taxonomy.blog.index+yaml";

                public const string Post = "application/vnd.vellum.taxonomy.blog+yaml";

                public const string PostsByAuthor = "application/vnd.vellum.taxonomy.blog.by.author+yaml";

                public const string PostsByEdition = "application/vnd.vellum.taxonomy.blog.by.edition+yaml";
            }
        }
    }
}