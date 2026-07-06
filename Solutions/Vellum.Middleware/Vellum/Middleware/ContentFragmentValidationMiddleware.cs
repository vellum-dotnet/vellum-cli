using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vellum.Abstractions;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Taxonomy;
using Vellum.Middleware.Abstractions;

namespace Vellum.Middleware;

/// <summary>
/// Pipeline middleware that validates every parsed content fragment against the interfaces its content
/// type and extensions map to — a frontmatter lint. All coercion failures are reported at once, with
/// their file paths, instead of surfacing one property access at a time during rendering.
/// </summary>
public class ContentFragmentValidationMiddleware
{
    /// <summary>
    /// The stage-data key (in the <see cref="PipelineStage.PostParsing"/> stage) under which the collected
    /// <see cref="CoercionFailure"/> list is published.
    /// </summary>
    public const string ValidationFailuresKey = "ContentFragmentCoercionFailures";

    private readonly IServiceCollection services;
    private readonly ILogger<ContentFragmentValidationMiddleware>? logger;

    /// <summary>
    /// Initializes a new instance of the ContentFragmentValidationMiddleware class.
    /// </summary>
    /// <param name="services">The service collection for dependency resolution.</param>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    public ContentFragmentValidationMiddleware(IServiceCollection services, ILogger<ContentFragmentValidationMiddleware>? logger = null)
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
            if (context.TaxonomyDocuments.Any())
            {
                ServiceProvider serviceProvider = this.services.BuildServiceProvider();
                IContentTypeInterfaceFactory? interfaceFactory = serviceProvider.GetService<IContentTypeInterfaceFactory>();

                if (interfaceFactory is not null)
                {
                    List<CoercionFailure> failures = [];

                    foreach (TaxonomyDocument document in context.TaxonomyDocuments)
                    {
                        foreach (ContentFragment fragment in document.ContentFragments)
                        {
                            failures.AddRange(Validate(fragment, interfaceFactory, serviceProvider));
                        }
                    }

                    ILogger? log = this.logger ?? serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<ContentFragmentValidationMiddleware>();

                    foreach (CoercionFailure failure in failures)
                    {
                        log?.LogWarning(
                            "Frontmatter validation: member '{MemberName}' value '{RawValue}' does not coerce to {TargetType} in '{FilePath}': {Reason}",
                            failure.MemberName,
                            failure.RawValue,
                            failure.TargetType.Name,
                            failure.FilePath,
                            failure.Reason);
                    }

                    context = context.SetStageData(PipelineStage.PostParsing, ValidationFailuresKey, failures);
                }
            }

            await next(context);
        };
    }

    private static List<CoercionFailure> Validate(ContentFragment fragment, IContentTypeInterfaceFactory interfaceFactory, IServiceProvider serviceProvider)
    {
        List<CoercionFailure> failures = [];
        List<Type> targetTypes = [];

        // A content type with no registered interface is fine — the fragment may never be duck-typed.
        // An extension with no registered interface is an error: ExtensionTypeFactory will throw at render time.
        Type? baseType = interfaceFactory.Resolve(fragment.ContentType);

        if (baseType is not null)
        {
            targetTypes.Add(baseType);
        }

        fragment.MetaData.TryGetValue("FilePath", out dynamic? filePath);

        foreach (string extension in fragment.Extensions)
        {
            Type? extensionType = interfaceFactory.Resolve(extension);

            if (extensionType is null)
            {
                failures.Add(new CoercionFailure(
                    "Extensions",
                    extension,
                    typeof(object),
                    filePath?.ToString(),
                    $"No content type interface is registered for extension content type '{extension}'. Register one via services.AddContentTypeInterface<TInterface>(\"{extension}\")."));
            }
            else if (!targetTypes.Contains(extensionType))
            {
                targetTypes.Add(extensionType);
            }
        }

        if (targetTypes.Count > 0)
        {
            DynamicContentFragment dynamicFragment = new(fragment, serviceProvider, [.. targetTypes]);
            failures.AddRange(dynamicFragment.Validate());
        }

        return failures;
    }
}