// <copyright file="ListOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content
{
    public class ListOptions
    {
        public ListOptions(bool draft, bool published)
        {
            this.Draft = draft;
            this.Published = published;
        }

        public bool Draft { get; }

        public bool Published { get; }
    }
}