using System;
using System.Collections.Generic;

namespace Vellum.Abstractions.Specs;

// Test-only model graph exercising deep and recursive coercion in DynamicContentFragment.

public class NestedOuter
{
    public NestedMiddle? Middle { get; set; }

    public string? Name { get; set; }
}

public class NestedMiddle
{
    public NestedLeaf? Leaf { get; set; }

    public List<string>? Tags { get; set; }
}

public class NestedLeaf
{
    public DateTime When { get; set; }

    public int Depth { get; set; }
}

/// <summary>
/// A POCO with an interface-typed property: a mixed graph shape the YAML round-trip cannot construct.
/// </summary>
public class MixedNode
{
    public IAuthoredArticle? Article { get; set; }
}

public interface IDeeplyNested
{
    NestedOuter Outer { get; set; }
}

public interface IAuthoredArticle
{
    string Title { get; set; }
}

public interface IArticleAuthor
{
    string FirstName { get; set; }

    IAuthoredArticle Featured { get; set; }

    IEnumerable<IAuthoredArticle> Posts { get; set; }
}

public interface IAuthoredRoot
{
    IArticleAuthor Author { get; set; }
}

public interface ILeafCollection
{
    IEnumerable<NestedLeaf> Leaves { get; set; }
}

public interface IMixedGraph
{
    MixedNode Node { get; set; }
}