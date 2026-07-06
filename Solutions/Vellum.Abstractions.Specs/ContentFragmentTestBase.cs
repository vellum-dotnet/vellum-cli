
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Formatting;

namespace Vellum.Abstractions.Specs;

public abstract class ContentFragmentTestBase : IDisposable
{
    protected ContentFragmentTestBase()
    {
        ServiceCollection services = new();

        IContentTransform passthroughTransform = Substitute.For<IContentTransform>();
        passthroughTransform.Apply(Arg.Any<string>()).Returns(call => call.Arg<string>());

        services.AddScoped<IContentFormatter, ContentFormatter>();
        services.AddScoped(_ => passthroughTransform);
        services.AddVellumContentExtensibility();
        services.AddWellKnownContentFragmentTypeFactories();
        services.AddWellKnownConverterFactories();

        this.ServiceProvider = services.BuildServiceProvider();
    }

    protected ServiceProvider ServiceProvider { get; }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.ServiceProvider.Dispose();
        }
    }

    protected ContentFragment CreateContentFragment(string markdownFileName, string contentBlockId = "Blogs")
    {
        ContentBlock contentBlock = new()
        {
            ContentType = WellKnown.ContentFragments.ContentTypes.BlogMarkdown,
            Id = contentBlockId,
            Spec = new ContentSpecification
            {
                Path = $"../../{markdownFileName}",
            },
        };

        FileInfo file = TestDocuments.GetMarkdownFile(markdownFileName);
        string content = File.ReadAllText(file.FullName);
        IContentFormatter contentFormatter = this.ServiceProvider.GetRequiredService<IContentFormatter>();

        return new MarkdownContentFragmentFactory(contentFormatter).Create(contentBlock, content, file.FullName);
    }
}