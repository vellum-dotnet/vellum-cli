// <copyright file="IAuthor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Vellum.Abstractions.Content;

public interface IAuthor : IContent, IAuthorId
{
    string Bio { get; set; }

    string Email { get; set; }

    EmploymentStatus EmploymentStatus { get; set; }

    string FirstName { get; set; }

    string FullName => $"{this.FirstName} {this.LastName}";

    string GitHub { get; set; }

    string HeroBio { get; set; }

    string JobTitle { get; set; }

    string LastName { get; set; }

    string LinkedIn { get; set; }

    bool Mvp { get; set; }

    IEnumerable<IBlogPost> Posts { get; set; }

    string ProfileHeroImageUrl { get; set; }

    string Twitter { get; set; }
}