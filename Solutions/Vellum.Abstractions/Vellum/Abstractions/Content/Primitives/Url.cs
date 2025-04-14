// <copyright file="Url.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Vellum.Abstractions.Content.Primitives;

public class Url
{
    private readonly string urlString;

    public Url(string urlString)
    {
        this.urlString = urlString;
    }

    public static implicit operator string(Url url) => url.ToString();

    public static explicit operator Url(string url) => new Url(url);

    public static bool AreEquivalent(Url first, Url second)
    {
        if (first == null || second == null)
        {
            return false;
        }

        if (first.IsEmpty() ^ second.IsEmpty())
        {
            return false;
        }

        if (first.IsEmpty() && second.IsEmpty())
        {
            return true;
        }

        bool areEquivalent = string.Equals(first.ToString(), second.ToString(), System.StringComparison.InvariantCultureIgnoreCase);

        if (!areEquivalent)
        {
            string firstAsString = first.ToString();
            string secondAsString = second.ToString();

            if (first.ToString().EndsWith("index.html", StringComparison.InvariantCultureIgnoreCase) ||
                first.ToString().EndsWith("index.htm", StringComparison.InvariantCultureIgnoreCase) ||
                first.ToString().EndsWith("default.html", StringComparison.InvariantCultureIgnoreCase) ||
                first.ToString().EndsWith("default.htm", StringComparison.InvariantCultureIgnoreCase))
            {
                int lastSlashIndex = first.ToString().LastIndexOf('/');

                firstAsString = first.ToString().Substring(0, lastSlashIndex);
            }

            if (second.ToString().ToLowerInvariant().EndsWith("index.html", StringComparison.InvariantCultureIgnoreCase) ||
                second.ToString().ToLowerInvariant().EndsWith("index.htm", StringComparison.InvariantCultureIgnoreCase) ||
                second.ToString().ToLowerInvariant().EndsWith("default.html", StringComparison.InvariantCultureIgnoreCase) ||
                second.ToString().ToLowerInvariant().EndsWith("default.htm", StringComparison.InvariantCultureIgnoreCase))
            {
                int lastSlashIndex = second.ToString().LastIndexOf('/');

                secondAsString = second.ToString().Substring(0, lastSlashIndex + 1);
            }

            areEquivalent = string.Equals(firstAsString, secondAsString, StringComparison.InvariantCultureIgnoreCase);
        }

        return areEquivalent;
    }

    public static string Normalize(string url)
    {
        if (url.EndsWith("/index.html", StringComparison.InvariantCultureIgnoreCase) ||
            url.EndsWith("/index.htm", StringComparison.InvariantCultureIgnoreCase) ||
            url.EndsWith("/default.html", StringComparison.InvariantCultureIgnoreCase) ||
            url.EndsWith("/default.htm", StringComparison.InvariantCultureIgnoreCase))
        {
            int lastSlashIndex = url.LastIndexOf('/');

            return url.Substring(0, lastSlashIndex);
        }

        if (url.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
        {
            int extension = url.LastIndexOf(".html");

            return url.Substring(0, extension);
        }

        return url;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(this.urlString);
    }

    public override string ToString()
    {
        return this.urlString;
    }
}