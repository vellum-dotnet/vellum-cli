using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Vellum.Abstractions.Content;

namespace Vellum.Abstractions.Specs;

/// <summary>
/// Locks in how far the duck-typing pipeline recurses: arbitrary-depth POCO graphs via the YAML
/// round-trip, recursive fragment-to-interface projection, per-element projection of fragment lists —
/// and the two documented limits (lists of mappings require a converter; POCOs with interface-typed
/// properties cannot ride the YAML path).
/// </summary>
[TestClass]
public class DeepNestingTests : ContentFragmentTestBase
{
    [TestMethod]
    public void Nested_pocos_coerce_recursively_through_multiple_levels()
    {
        ContentFragment contentFragment = NewFragment(new Dictionary<string, dynamic>
        {
            ["Outer"] = new Dictionary<object, object>
            {
                ["Name"] = "outer",
                ["Middle"] = new Dictionary<object, object>
                {
                    ["Tags"] = new List<object> { "one", "two" },
                    ["Leaf"] = new Dictionary<object, object>
                    {
                        ["When"] = "2026-07-06",
                        ["Depth"] = "3",
                    },
                },
            },
        });

        DynamicContentFragment dynamicFragment = new(contentFragment, this.ServiceProvider, typeof(IDeeplyNested));
        IDeeplyNested probe = dynamicFragment.ActLike<IDeeplyNested>();

        probe.Outer.Name.ShouldBe("outer");
        probe.Outer.Middle.ShouldNotBeNull();
        probe.Outer.Middle.Tags.ShouldBe(["one", "two"]);
        probe.Outer.Middle.Leaf.ShouldNotBeNull();
        probe.Outer.Middle.Leaf.When.ShouldBe(new DateTime(2026, 7, 6));
        probe.Outer.Middle.Leaf.Depth.ShouldBe(3);
    }

    [TestMethod]
    public void Fragment_graphs_project_recursively_across_multiple_levels()
    {
        ContentFragment featured = NewFragment(new Dictionary<string, dynamic>
        {
            ["Title"] = "featured article",
        });

        ContentFragment author = NewFragment(new Dictionary<string, dynamic>
        {
            ["FirstName"] = "Howard",
            ["Featured"] = featured,
        });

        ContentFragment root = NewFragment(new Dictionary<string, dynamic>
        {
            ["Author"] = author,
        });

        DynamicContentFragment dynamicFragment = new(root, this.ServiceProvider, typeof(IAuthoredRoot));
        IAuthoredRoot probe = dynamicFragment.ActLike<IAuthoredRoot>();

        // root fragment -> IArticleAuthor -> nested fragment -> IAuthoredArticle: two levels of projection.
        probe.Author.FirstName.ShouldBe("Howard");
        probe.Author.Featured.ShouldBeAssignableTo<IAuthoredArticle>();
        probe.Author.Featured.Title.ShouldBe("featured article");
    }

    [TestMethod]
    public void Lists_of_fragments_project_each_element_onto_the_declared_interface()
    {
        ContentFragment postOne = NewFragment(new Dictionary<string, dynamic> { ["Title"] = "post one" });
        ContentFragment postTwo = NewFragment(new Dictionary<string, dynamic> { ["Title"] = "post two" });

        ContentFragment author = NewFragment(new Dictionary<string, dynamic>
        {
            ["FirstName"] = "Howard",
            ["Posts"] = new List<object> { postOne, postTwo },
        });

        DynamicContentFragment dynamicFragment = new(author, this.ServiceProvider, typeof(IArticleAuthor));
        IArticleAuthor probe = dynamicFragment.ActLike<IArticleAuthor>();

        List<IAuthoredArticle> posts = probe.Posts.ToList();

        posts.Count.ShouldBe(2);
        posts[0].Title.ShouldBe("post one");
        posts[1].Title.ShouldBe("post two");
    }

    [TestMethod]
    public void Typed_lists_of_fragments_project_each_element_onto_the_declared_interface()
    {
        // Code-constructed metadata (e.g. taxonomy extensions) holds List<ContentFragment>, not the
        // List<object> the YAML parser produces; both shapes must project their elements.
        ContentFragment post = NewFragment(new Dictionary<string, dynamic> { ["Title"] = "typed post" });

        ContentFragment author = NewFragment(new Dictionary<string, dynamic>
        {
            ["FirstName"] = "Howard",
            ["Posts"] = new List<ContentFragment> { post },
        });

        DynamicContentFragment dynamicFragment = new(author, this.ServiceProvider, typeof(IArticleAuthor));
        IArticleAuthor probe = dynamicFragment.ActLike<IArticleAuthor>();

        IAuthoredArticle first = probe.Posts.ShouldHaveSingleItem();
        first.Title.ShouldBe("typed post");
    }

    [TestMethod]
    public void Lists_of_nested_mappings_require_a_registered_converter()
    {
        // Documented limit: unlike a scalar nested mapping (which round-trips through YAML into the
        // declared POCO), a *list* of mappings must have a member-named converter, and fails loudly.
        ContentFragment contentFragment = NewFragment(new Dictionary<string, dynamic>
        {
            ["Leaves"] = new List<object>
            {
                new Dictionary<object, object> { ["When"] = "2026-07-06", ["Depth"] = "1" },
            },
        });

        DynamicContentFragment dynamicFragment = new(contentFragment, this.ServiceProvider, typeof(ILeafCollection));
        ILeafCollection probe = dynamicFragment.ActLike<ILeafCollection>();

        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => probe.Leaves);

        exception.Message.ShouldContain("leaves+converter");
    }

    [TestMethod]
    public void Pocos_with_interface_typed_properties_default_and_report_a_failure()
    {
        // Documented limit: the YAML round-trip cannot construct an interface-typed property, so the
        // member defaults to null and the failure is reported (visible through Validate and the logger).
        ContentFragment contentFragment = NewFragment(new Dictionary<string, dynamic>
        {
            ["Node"] = new Dictionary<object, object>
            {
                ["Article"] = new Dictionary<object, object> { ["Title"] = "unreachable" },
            },
        });

        DynamicContentFragment dynamicFragment = new(contentFragment, this.ServiceProvider, typeof(IMixedGraph));

        CoercionFailure failure = dynamicFragment.Validate().ShouldHaveSingleItem();
        failure.MemberName.ShouldBe("Node");
        failure.TargetType.ShouldBe(typeof(MixedNode));

        IMixedGraph probe = dynamicFragment.ActLike<IMixedGraph>();
        probe.Node.ShouldBeNull();
    }

    private static ContentFragment NewFragment(Dictionary<string, dynamic> metaData)
    {
        return new ContentFragment
        {
            Id = "DeepNesting",
            ContentType = WellKnown.ContentFragments.ContentTypes.BlogMarkdown,
            Hash = "not-a-real-hash",
            MetaData = metaData,
        };
    }
}