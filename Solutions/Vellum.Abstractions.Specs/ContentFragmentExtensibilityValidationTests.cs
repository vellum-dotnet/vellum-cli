
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Shouldly;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Extensions;

namespace Vellum.Abstractions.Specs;

[TestClass]
public class ContentFragmentExtensibilityValidationTests : ContentFragmentTestBase
{
    [TestMethod]
    public void An_unregistered_extension_content_type_fails_with_a_descriptive_exception()
    {
        ContentFragment contentFragment = this.CreateContentFragment("blog-with-unknown-extension.md");
        IExtensionTypeFactory extensionTypeFactory = this.ServiceProvider.GetRequiredService<IExtensionTypeFactory>();

        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => extensionTypeFactory.Create(contentFragment));

        exception.Message.ShouldContain("application/vnd.vellum.content.unknown+md");
        exception.Message.ShouldContain((string)contentFragment.MetaData["FilePath"]);
    }

    [TestMethod]
    public void Null_and_missing_metadata_values_coerce_to_sensible_defaults()
    {
        ContentFragment contentFragment = this.CreateContentFragment("blog-with-null-values.md");
        IExtensionTypeFactory extensionTypeFactory = this.ServiceProvider.GetRequiredService<IExtensionTypeFactory>();

        object? extendedBlogPost = extensionTypeFactory.Create(contentFragment);

        IBlogPost blogPost = extendedBlogPost.ShouldBeAssignableTo<IBlogPost>()!;

        // Null YAML value on a nullable string property.
        blogPost.HeaderImageUrl.ShouldBeNull();

        // Null YAML value on an extension interface property.
        ((ISeries)blogPost).Series.ShouldBeNull();

        // Key entirely absent from the frontmatter on a value-type property.
        ((IPromotions)blogPost).Promote.ShouldBeFalse();
    }

    [TestMethod]
    public void The_extension_type_factory_names_the_unresolvable_content_type_without_touching_the_file_system()
    {
        IContentTypeInterfaceFactory interfaceFactory = Substitute.For<IContentTypeInterfaceFactory>();
        interfaceFactory.Resolve(Arg.Any<string>()).Returns((Type?)null);

        ExtensionTypeFactory extensionTypeFactory = new(this.ServiceProvider, interfaceFactory);

        ContentFragment contentFragment = new()
        {
            Id = "Blogs",
            ContentType = WellKnown.ContentFragments.ContentTypes.BlogMarkdown,
            Hash = "not-a-real-hash",
            Extensions = [WellKnown.ContentFragments.ContentTypes.Series],
        };

        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => extensionTypeFactory.Create(contentFragment));

        exception.Message.ShouldContain(WellKnown.ContentFragments.ContentTypes.BlogMarkdown);
        exception.Message.ShouldContain("Blogs");
        exception.Message.ShouldContain("AddContentTypeInterface");
    }
}