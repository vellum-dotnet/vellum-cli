
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Vellum.Abstractions.Content;

namespace Vellum.Abstractions.Specs;

[TestClass]
public class MarkdownContentFragmentFactoryTests : ContentFragmentTestBase
{
    [TestMethod]
    public void Converting_a_markdown_document_into_a_content_fragment()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);

        contentFragment.ContentType.ShouldBe(WellKnown.ContentFragments.ContentTypes.BlogMarkdown);
        contentFragment.Hash.ShouldBe(AzureSynapseBlogPostExpectations.Hash);
        contentFragment.Id.ShouldBe("Blogs");
        contentFragment.Position.ShouldBe(0);
        contentFragment.Date.ShouldBe(new DateTime(2020, 7, 15, 6, 30, 0));
        contentFragment.PublicationStatus.ShouldBe(PublicationStatus.Published);
    }

    [TestMethod]
    public void Content_fragment_metadata_contains_the_frontmatter_values()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);

        ((string)contentFragment.MetaData["Title"]).ShouldBe(AzureSynapseBlogPostExpectations.Title);
        ((string)contentFragment.MetaData["Slug"]).ShouldBe(AzureSynapseBlogPostExpectations.Slug);
        ((string)contentFragment.MetaData["Author"]).ShouldBe(AzureSynapseBlogPostExpectations.Author);
        ((string)contentFragment.MetaData["HeaderImageUrl"]).ShouldBe(AzureSynapseBlogPostExpectations.HeaderImageUrl);
        ((string)contentFragment.MetaData["Excerpt"]).ShouldBe(AzureSynapseBlogPostExpectations.Excerpt);

        FileInfo filePath = new((string)contentFragment.MetaData["FilePath"]);
        filePath.Name.ShouldBe(AzureSynapseBlogPostExpectations.MarkdownFileName);
        filePath.Directory?.Name.ShouldBe("MarkdownDocuments");
    }

    [TestMethod]
    public void Content_fragment_metadata_contains_the_categories()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);

        List<object> categories = contentFragment.MetaData["Category"];

        categories.Cast<string>().ShouldBe(AzureSynapseBlogPostExpectations.Categories);
    }

    [TestMethod]
    public void Content_fragment_metadata_contains_the_tags()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);

        List<object> tags = contentFragment.MetaData["Tags"];

        tags.Cast<string>().ShouldBe(AzureSynapseBlogPostExpectations.Tags);
    }

    [TestMethod]
    public void Content_fragment_metadata_contains_the_faqs()
    {
        ContentFragment contentFragment = this.CreateContentFragment(AzureSynapseBlogPostExpectations.MarkdownFileName);

        List<KeyValuePair<string, string>> expected =
        [
            new("Q", AzureSynapseBlogPostExpectations.FaqOneQuestion),
            new("A", AzureSynapseBlogPostExpectations.FaqOneAnswer),
            new("Q", AzureSynapseBlogPostExpectations.FaqTwoQuestion),
            new("A", AzureSynapseBlogPostExpectations.FaqTwoAnswer),
        ];

        List<object> faqs = contentFragment.MetaData["FAQs"];

        IEnumerable<KeyValuePair<string, string>> actual = faqs.Cast<Dictionary<object, object>>()
            .SelectMany(x => x)
            .Select(x => new KeyValuePair<string, string>((string)x.Key, (string)x.Value));

        actual.ShouldBe(expected);
    }
}