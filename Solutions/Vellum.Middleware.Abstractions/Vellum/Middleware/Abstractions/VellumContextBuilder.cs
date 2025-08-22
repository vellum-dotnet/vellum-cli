using System.Collections.Immutable;
using Vellum.Abstractions.Taxonomy;

namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Efficient builder for creating VellumContext instances with mutable operations.
/// This avoids the O(n²) complexity of repeated immutable updates.
/// </summary>
public class VellumContextBuilder
{
    private PipelineStage currentStage = PipelineStage.Initialization;
    private readonly Dictionary<string, object?> items = new();
    private readonly Dictionary<PipelineStage, Dictionary<string, object?>> stageData = new();
    private readonly Dictionary<string, object?> stageMetrics = new();
    private readonly List<ContentFile> contentFiles = [];
    private readonly List<ProcessedContent> processedContent = [];
    private SiteDetails? siteDetails = null;
    private List<TaxonomyDocument> taxonomyDocuments = [];
    private SiteContext? siteContext = null;

    /// <summary>
    /// Initializes a new instance of the VellumContextBuilder class.
    /// </summary>
    public VellumContextBuilder()
    {
    }

    /// <summary>
    /// Creates a builder from an existing context.
    /// </summary>
    /// <param name="context">The existing context to copy from.</param>
    /// <returns>A new builder with the context's data.</returns>
    public static VellumContextBuilder From(VellumContext context)
    {
        VellumContextBuilder builder = new()
        {
            currentStage = context.CurrentStage
        };

        // Copy items
        foreach (KeyValuePair<string, object?> item in context.Items)
        {
            builder.items[item.Key] = item.Value;
        }

        // Copy stage data
        foreach (KeyValuePair<PipelineStage, ImmutableDictionary<string, object?>> stage in context.StageData)
        {
            Dictionary<string, object?> stageDict = new();
            foreach (KeyValuePair<string, object?> item in stage.Value)
            {
                stageDict[item.Key] = item.Value;
            }
            builder.stageData[stage.Key] = stageDict;
        }

        // Copy metrics
        foreach (KeyValuePair<string, object?> metric in context.StageMetrics)
        {
            builder.stageMetrics[metric.Key] = metric.Value;
        }

        // Copy content files
        builder.contentFiles.AddRange(context.ContentFiles);

        // Copy processed content
        builder.processedContent.AddRange(context.ProcessedContent);

        builder.siteDetails = context.SiteDetails;

        builder.taxonomyDocuments.AddRange(context.TaxonomyDocuments);

        return builder;
    }

    /// <summary>
    /// Sets the current pipeline stage.
    /// </summary>
    /// <param name="stage">The pipeline stage.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder SetCurrentStage(PipelineStage stage)
    {
        this.currentStage = stage;
        return this;
    }

    /// <summary>
    /// Sets an item in the context.
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <param name="value">The item value.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder SetItem(string key, object? value)
    {
        this.items[key] = value;
        return this;
    }

    /// <summary>
    /// Sets data for a specific pipeline stage.
    /// </summary>
    /// <param name="stage">The pipeline stage.</param>
    /// <param name="key">The data key.</param>
    /// <param name="value">The data value.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder SetStageData(PipelineStage stage, string key, object? value)
    {
        if (!this.stageData.ContainsKey(stage))
        {
            this.stageData[stage] = new Dictionary<string, object?>();
        }

        this.stageData[stage][key] = value;
        return this;
    }

    /// <summary>
    /// Sets a metric value.
    /// </summary>
    /// <param name="key">The metric key.</param>
    /// <param name="value">The metric value.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder SetMetric(string key, object? value)
    {
        this.stageMetrics[key] = value;
        return this;
    }

    /// <summary>
    /// Sets the content files collection.
    /// </summary>
    /// <param name="files">The content files.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder SetContentFiles(IEnumerable<ContentFile> files)
    {
        this.contentFiles.Clear();
        this.contentFiles.AddRange(files);
        return this;
    }

    /// <summary>
    /// Adds a single content file.
    /// </summary>
    /// <param name="file">The content file to add.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder AddContentFile(ContentFile file)
    {
        this.contentFiles.Add(file);
        return this;
    }

    /// <summary>
    /// Sets the processed content collection.
    /// </summary>
    /// <param name="content">The processed content.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder SetProcessedContent(IEnumerable<ProcessedContent> content)
    {
        this.processedContent.Clear();
        this.processedContent.AddRange(content);
        return this;
    }

    /// <summary>
    /// Adds a single processed content item.
    /// </summary>
    /// <param name="content">The processed content to add.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public VellumContextBuilder AddProcessedContent(ProcessedContent content)
    {
        this.processedContent.Add(content);
        return this;
    }

    /// <summary>
    /// Builds an immutable VellumContext from the builder's current state.
    /// </summary>
    /// <returns>A new immutable VellumContext.</returns>
    public VellumContext Build()
    {
        // Convert stage data to immutable
        ImmutableDictionary<PipelineStage, ImmutableDictionary<string, object?>>.Builder stageDataBuilder =
            ImmutableDictionary.CreateBuilder<PipelineStage, ImmutableDictionary<string, object?>>();

        foreach (KeyValuePair<PipelineStage, Dictionary<string, object?>> stage in this.stageData)
        {
            stageDataBuilder[stage.Key] = stage.Value.ToImmutableDictionary();
        }

        return new VellumContext
        {
            ContentFiles = this.contentFiles.ToImmutableList(),
            CurrentStage = this.currentStage,
            Items = this.items.ToImmutableDictionary(),
            ProcessedContent = this.processedContent.ToImmutableList(),
            SiteContext = this.siteContext,
            SiteDetails = this.siteDetails,
            StageData = stageDataBuilder.ToImmutable(),
            StageMetrics = this.stageMetrics.ToImmutableDictionary(),
            TaxonomyDocuments = this.taxonomyDocuments.ToImmutableList(),
        };
    }

    public VellumContextBuilder AddSiteDetails(SiteDetails details)
    {
        this.siteDetails = details;

        return this;
    }

    public VellumContextBuilder AddTaxonomyDocuments(List<TaxonomyDocument> documents)
    {
        this.taxonomyDocuments = documents;

        return this;
    }

    public VellumContextBuilder AddSiteContext(SiteContext context)
    {
        this.siteContext = context;

        return this;
    }
}