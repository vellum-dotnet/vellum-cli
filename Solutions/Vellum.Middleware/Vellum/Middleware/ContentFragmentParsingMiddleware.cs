using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.Transformations;
using Vellum.Abstractions.Taxonomy;
using Vellum.Middleware.Abstractions;
using Vellum.Middleware.Repositories;

namespace Vellum.Middleware;

public class ContentFragmentParsingMiddleware
{
    private readonly IServiceCollection services;
    private readonly ILogger<ContentFragmentParsingMiddleware>? logger;

    /// <summary>
    /// Initializes a new instance of the ContentFragmentParsingMiddleware class.
    /// </summary>
    /// <param name="services">The service collection for dependency resolution.</param>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    public ContentFragmentParsingMiddleware(IServiceCollection services, ILogger<ContentFragmentParsingMiddleware>? logger = null)
    {
        this.services = services;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a middleware delegate for the pipeline.
    /// </summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>The middleware delegate.</returns>
    public RequestDelegate InvokeAsync(RequestDelegate next)
    {
        return async context =>
        {
            VellumContextBuilder builder = VellumContextBuilder.From(context);

            if (context.TaxonomyDocuments.Any())
            {
                ServiceProvider serviceProvider = this.services.BuildServiceProvider();

                InMemoryContentFragmentRepository repository = new();
                repository.Initialize(context);

                serviceProvider = this.services.AddSingleton<IContentFragmentRepository>(repository).BuildServiceProvider();

                List<IContentFragmentTransformer> transformers = serviceProvider.GetServices<IContentFragmentTransformer>().ToList();
                ContentFragmentTransformationPipeline? pipeline = null;

                if (transformers.Any())
                {
                    pipeline = new ContentFragmentTransformationPipeline(transformers, serviceProvider);
                }

                List<TaxonomyDocument> transformedDocuments = [];

                foreach (TaxonomyDocument document in context.TaxonomyDocuments)
                {
                    if (pipeline != null && document.ContentFragments.Any())
                    {
                        List<ContentFragment> transformedFragments = [];

                        foreach (ContentFragment fragment in document.ContentFragments)
                        {
                            ContentFragment transformedFragment = pipeline.Process(fragment);
                            transformedFragments.Add(transformedFragment);
                        }

                        TaxonomyDocument transformedDocument = document with
                        {
                            ContentFragments = transformedFragments
                        };

                        transformedDocuments.Add(transformedDocument);
                    }
                    else
                    {
                        transformedDocuments.Add(document);
                    }
                }

                builder.AddTaxonomyDocuments(transformedDocuments);
            }

            VellumContext updatedContext = builder.Build();

            await next(updatedContext);
        };
    }
}