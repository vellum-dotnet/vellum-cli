// <copyright file="SiteTaxonomyParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Vellum.Abstractions.Content.Primitives;

namespace Vellum.Abstractions.Taxonomy;

public class SiteTaxonomyParser
{
    public NavigationNode Parse(IReadOnlyCollection<TaxonomyDocument> pages)
    {
        NavigationNode root = pages.Where(x => x.Navigation?.Parent is null).Select(x => new NavigationNode
        {
            Description = x.MetaData!.Description,
            Header = x.Navigation?.Header,
            Footer = x.Navigation?.Footer,
            Url = x.Navigation?.Url,
            Rank = x.Navigation?.Rank,
            Title = x.Title,
        }).First();

        root.Children = pages.Where(x => Url.AreEquivalent(x.Navigation!.Parent!, root.Url!)).Select(x => new NavigationNode
        {
            Description = x.MetaData!.Description,
            Header = x.Navigation!.Header,
            Footer = x.Navigation.Footer,
            Url = x.Navigation.Url,
            Rank = x.Navigation.Rank,
            Title = x.Title,
        }).OrderBy(x => x.Rank).ToList();

        foreach (TaxonomyDocument page in pages)
        {
            NavigationNode? parentNode = root.Children.Find(x => Url.AreEquivalent(x.Url!, page.Navigation!.Parent!));
            parentNode?.Children.Add(new NavigationNode
            {
                Description = page.MetaData!.Description,
                Header = page.Navigation!.Header,
                Footer = page.Navigation.Footer,
                Url = page.Navigation.Url,
                Rank = page.Navigation.Rank,
                Title = page.Title,
            });
        }

        var sorted = root.Children.OrderBy(x => x.Rank).ToList();

        foreach (NavigationNode node in sorted)
        {
            node.Children = node.Children.OrderBy(x => x.Rank).ToList();
        }

        root.Children = sorted.ToList();

        return root;
    }
}