using System;
using System.Collections.Generic;
using ImpromptuInterface;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Extensions;

namespace Vellum.Abstractions.Specs;

[TestClass]
public class DynamicContentFragmentTests : ContentFragmentTestBase
{
    [TestMethod]
    public void Repeated_reads_return_the_same_coerced_instance()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);
        ContentFragmentTypeFactory<IBlogPost>? typeFactory = this.ServiceProvider.GetContent<ContentFragmentTypeFactory<IBlogPost>>(contentFragment.ContentType.AsContentFragmentFactory());

        typeFactory.ShouldNotBeNull();

        IBlogPost blogPost = typeFactory.Create(contentFragment);

        ReferenceEquals(blogPost.Faqs, blogPost.Faqs).ShouldBeTrue();
        ReferenceEquals(blogPost.Tags, blogPost.Tags).ShouldBeTrue();
    }

    [TestMethod]
    public void Nested_frontmatter_objects_coerce_to_the_declared_poco()
    {
        ContentFragment contentFragment = NewFragment(new Dictionary<string, dynamic>
        {
            ["Dates"] = new Dictionary<object, object>
            {
                ["Date"] = "2026-07-06",
                ["Ordinal"] = "6th July 2026",
                ["Short"] = "06 Jul 2026",
            },
        });

        DynamicContentFragment dynamicFragment = new(contentFragment, this.ServiceProvider, typeof(IBlogPost));
        IBlogPost blogPost = dynamicFragment.ActLike<IBlogPost>();

        blogPost.Dates.ShouldNotBeNull();
        blogPost.Dates.Date.ShouldBe(new DateTime(2026, 7, 6));
        blogPost.Dates.Ordinal.ShouldBe("6th July 2026");
        blogPost.Dates.Short.ShouldBe("06 Jul 2026");
    }

    [TestMethod]
    public void Fragment_valued_members_duck_type_recursively()
    {
        ContentFragment author = new()
        {
            Id = "authors/howard",
            ContentType = "application/vnd.vellum.content.author+md",
            Hash = "not-a-real-hash",
            MetaData = new Dictionary<string, dynamic>
            {
                ["FirstName"] = "Howard",
                ["LastName"] = "van Rooijen",
            },
        };

        ContentFragment contentFragment = NewFragment(new Dictionary<string, dynamic>
        {
            ["Author"] = author,
        });

        DynamicContentFragment dynamicFragment = new(contentFragment, this.ServiceProvider, typeof(IAuthoredContent));
        IAuthoredContent content = dynamicFragment.ActLike<IAuthoredContent>();

        content.Author.ShouldBeAssignableTo<IAuthor>();
        content.Author.FirstName.ShouldBe("Howard");
        content.Author.LastName.ShouldBe("van Rooijen");
    }

    [TestMethod]
    public void Unknown_members_return_null_by_default()
    {
        dynamic dynamicFragment = new DynamicContentFragment(NewFragment(new Dictionary<string, dynamic>()), this.ServiceProvider, typeof(IBlogPost));

        ((object?)dynamicFragment.DefinitelyNotAMember).ShouldBeNull();
    }

    [TestMethod]
    public void Strict_member_access_fails_the_binding_for_unknown_members()
    {
        ServiceCollection services = new();
        services.AddVellumContentExtensibility(options => options.StrictMemberAccess = true);

        using ServiceProvider strictProvider = services.BuildServiceProvider();

        dynamic dynamicFragment = new DynamicContentFragment(NewFragment(new Dictionary<string, dynamic>()), strictProvider, typeof(IBlogPost));

        Should.Throw<RuntimeBinderException>(() => _ = dynamicFragment.DefinitelyNotAMember);
    }

    [TestMethod]
    public void Strict_member_access_still_defaults_members_the_interfaces_declare()
    {
        ServiceCollection services = new();
        services.AddVellumContentExtensibility(options => options.StrictMemberAccess = true);

        using ServiceProvider strictProvider = services.BuildServiceProvider();

        dynamic dynamicFragment = new DynamicContentFragment(NewFragment(new Dictionary<string, dynamic>()), strictProvider, typeof(IPromotions));

        ((object)dynamicFragment.Promote).ShouldBe(false);
    }

    [TestMethod]
    public void Validate_reports_every_member_that_fails_to_coerce()
    {
        ContentFragment contentFragment = NewFragment(new Dictionary<string, dynamic>
        {
            ["Promote"] = "definitely",
            ["FilePath"] = "C:/site/blog/post.md",
        });

        DynamicContentFragment dynamicFragment = new(contentFragment, this.ServiceProvider, typeof(IPromotions));

        IReadOnlyList<CoercionFailure> failures = dynamicFragment.Validate();

        CoercionFailure failure = failures.ShouldHaveSingleItem();
        failure.MemberName.ShouldBe("Promote");
        failure.RawValue.ShouldBe("definitely");
        failure.TargetType.ShouldBe(typeof(bool));
        failure.FilePath.ShouldBe("C:/site/blog/post.md");
    }

    [TestMethod]
    public void Validate_returns_no_failures_for_a_well_formed_fragment()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);

        DynamicContentFragment dynamicFragment = new(contentFragment, this.ServiceProvider, typeof(IBlogPost));

        dynamicFragment.Validate().ShouldBeEmpty();
    }

    private static ContentFragment NewFragment(Dictionary<string, dynamic> metaData)
    {
        return new ContentFragment
        {
            Id = "Blogs",
            ContentType = WellKnown.ContentFragments.ContentTypes.BlogMarkdown,
            Hash = "not-a-real-hash",
            MetaData = metaData,
        };
    }
}