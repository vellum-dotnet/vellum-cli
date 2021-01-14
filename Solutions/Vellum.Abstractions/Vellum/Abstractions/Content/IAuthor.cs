// <copyright file="IAuthor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System.Collections.Generic;

    public interface IAuthor : IContent
    {
        string Bio { get; set; }

        string Email { get; set; }

        EmploymentStatus EmploymentStatus { get; set; }

        string FirstName { get; set; }

        string FullName { get; }

        string GitHub { get; set; }

        string HeroBio { get; set; }

        string JobTitle { get; set; }

        string LastName { get; set; }

        string LinkedIn { get; set; }

        bool Mvp { get; set; }

        IEnumerable<IBlogPost> Posts { get; set; }

        string ProfileHeroImageUrl { get; set; }

        string Twitter { get; set; }

        string UserName { get; set; }
    }
}