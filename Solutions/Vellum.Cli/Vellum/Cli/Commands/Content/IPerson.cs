// <copyright file="IPerson.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    public interface IPerson
    {
        string FirstName { get; set; }

        string LastName { get; set;  }
    }
}