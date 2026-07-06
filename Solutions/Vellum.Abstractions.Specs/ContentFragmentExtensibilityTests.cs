
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Extensions;

namespace Vellum.Abstractions.Specs;

[TestClass]
public class ContentFragmentExtensibilityTests : ContentFragmentTestBase
{
    private const string MarkdownFileName = "blog-with-extensions.md";

    [TestMethod]
    public void A_markdown_file_with_extensions_is_converted_into_a_content_fragment_with_extensions()
    {
        ContentFragment contentFragment = this.CreateContentFragment(MarkdownFileName);

        contentFragment.ContentType.ShouldBe(WellKnown.ContentFragments.ContentTypes.BlogMarkdown);
        contentFragment.Hash.ShouldBe("58754666f4d7b6578f70dea23f2a24a3aa2d6771a91e8baed08874501557e9b2");
        contentFragment.Id.ShouldBe("Blogs");
        contentFragment.Position.ShouldBe(0);
        contentFragment.Date.ShouldBe(new DateTime(2022, 11, 5, 6, 30, 0));
        contentFragment.PublicationStatus.ShouldBe(PublicationStatus.Published);

        contentFragment.Extensions.ShouldBe(
            [
                "application/vnd.vellum.content.series+md",
                "application/vnd.vellum.content.promotion+md",
            ],
            ignoreOrder: true);
    }

    [TestMethod]
    public void A_content_fragment_with_extensions_generates_a_dynamic_type_implementing_the_extension_interfaces()
    {
        ContentFragment contentFragment = this.CreateContentFragment(MarkdownFileName);
        IExtensionTypeFactory? extensionTypeFactory = this.ServiceProvider.GetService<IExtensionTypeFactory>();

        extensionTypeFactory.ShouldNotBeNull();
        contentFragment.Extensions.Count().ShouldBeGreaterThan(0);

        object? extendedBlogPost = extensionTypeFactory.Create(contentFragment);

        IBlogPost blogPost = extendedBlogPost.ShouldBeAssignableTo<IBlogPost>()!;

        // The dynamic proxy implements the extension interfaces, so pattern matching works.
        IPromotions promotions = blogPost.ShouldBeAssignableTo<IPromotions>()!;
        promotions.Promote.ShouldBeTrue();

        ISeries series = blogPost.ShouldBeAssignableTo<ISeries>()!;
        series.PartTitle.ShouldBe("Part One");
        series.Series.ShouldBe("Blog Post Series");

        // Standard casting works too.
        ((IPromotions)blogPost).Promote.ShouldBeTrue();
        ((ISeries)blogPost).PartTitle.ShouldBe("Part One");
        ((ISeries)blogPost).Series.ShouldBe("Blog Post Series");
    }
}