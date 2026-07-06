
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Extensions;

namespace Vellum.Abstractions.Specs;

[TestClass]
public class ContentFragmentTypeFactoryTests : ContentFragmentTestBase
{
    private IBlogPost CreateBlogPost()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);
        ContentFragmentTypeFactory<IBlogPost>? typeFactory = this.ServiceProvider.GetContent<ContentFragmentTypeFactory<IBlogPost>>(contentFragment.ContentType.AsContentFragmentFactory());

        typeFactory.ShouldNotBeNull();

        return typeFactory.Create(contentFragment);
    }

    [TestMethod]
    public void Converting_a_content_fragment_into_a_blog_post()
    {
        IBlogPost blogPost = this.CreateBlogPost();

        string bodyHtml = TestDocuments.GetHtmlContent(AzureSynapseBlogPostExpectations.HtmlFileName);

        blogPost.Title.ShouldBe(AzureSynapseBlogPostExpectations.Title);
        blogPost.Slug.ShouldBe(AzureSynapseBlogPostExpectations.Slug);
        blogPost.HeaderImageUrl.ShouldBe(AzureSynapseBlogPostExpectations.HeaderImageUrl);
        blogPost.Excerpt.ShouldBe(AzureSynapseBlogPostExpectations.Excerpt);
        blogPost.Date.ShouldBe(new DateTime(2020, 7, 15, 6, 30, 0));
        blogPost.PublicationStatus.ShouldBe(PublicationStatus.Published);
        blogPost.Body.NormalizeLineEndings().ShouldBe(bodyHtml.NormalizeLineEndings());
    }

    [TestMethod]
    public void Blog_post_categories_are_typed_as_strings()
    {
        IBlogPost blogPost = this.CreateBlogPost();

        blogPost.Category.ShouldBe(AzureSynapseBlogPostExpectations.Categories);
    }

    [TestMethod]
    public void Blog_post_tags_are_typed_as_strings()
    {
        IBlogPost blogPost = this.CreateBlogPost();

        blogPost.Tags.ShouldBeAssignableTo<List<string>>();
        blogPost.Tags.ShouldBe(AzureSynapseBlogPostExpectations.Tags);
    }

    [TestMethod]
    public void Blog_post_faqs_are_converted_to_question_answer_tuples()
    {
        IBlogPost blogPost = this.CreateBlogPost();

        List<(string Question, string Answer)> expected =
        [
            (AzureSynapseBlogPostExpectations.FaqOneQuestion, AzureSynapseBlogPostExpectations.FaqOneAnswer),
            (AzureSynapseBlogPostExpectations.FaqTwoQuestion, AzureSynapseBlogPostExpectations.FaqTwoAnswer),
        ];

        blogPost.Faqs.ShouldBeAssignableTo<IEnumerable<(string, string)>>();
        blogPost.Faqs.ShouldBe(expected);
    }
}