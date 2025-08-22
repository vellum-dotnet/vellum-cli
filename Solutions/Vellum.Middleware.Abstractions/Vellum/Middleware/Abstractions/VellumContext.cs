using System.Collections.Immutable;
using Vellum.Abstractions.Taxonomy;

namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Immutable context that flows through the pipeline stages.
/// </summary>
public record VellumContext
{
    public SiteDetails? SiteDetails { get; init; } = null;

    public ImmutableList<TaxonomyDocument> TaxonomyDocuments { get; init; } = [];

    /// <summary>
    /// Gets the current pipeline stage.
    /// </summary>
    public PipelineStage CurrentStage { get; init; } = PipelineStage.Initialization;

    /// <summary>
    /// Gets arbitrary items that can be used to share data between middleware.
    /// </summary>
    public ImmutableDictionary<string, object?> Items { get; init; } = ImmutableDictionary<string, object?>.Empty;

    /// <summary>
    /// Gets stage-specific data bags for storing data per pipeline stage.
    /// </summary>
    public ImmutableDictionary<PipelineStage, ImmutableDictionary<string, object?>> StageData { get; init; } = ImmutableDictionary<PipelineStage, ImmutableDictionary<string, object?>>.Empty;

    /// <summary>
    /// Gets metrics collected during pipeline execution.
    /// </summary>
    public ImmutableDictionary<string, object?> StageMetrics { get; init; } = ImmutableDictionary<string, object?>.Empty;

    /// <summary>
    /// Gets the content files discovered during the Discovery stage.
    /// </summary>
    public ImmutableList<ContentFile> ContentFiles { get; init; } = ImmutableList<ContentFile>.Empty;

    /// <summary>
    /// Gets the processed content after transformation stages.
    /// </summary>
    public ImmutableList<ProcessedContent> ProcessedContent { get; init; } = ImmutableList<ProcessedContent>.Empty;

    public SiteContext? SiteContext { get; set; }

    /// <summary>
    /// Sets data for a specific pipeline stage.
    /// </summary>
    /// <param name="stage">The pipeline stage.</param>
    /// <param name="key">The data key.</param>
    /// <param name="value">The data value.</param>
    /// <returns>A new VellumContext with the updated stage data.</returns>
    public VellumContext SetStageData(PipelineStage stage, string key, object? value)
    {
        ImmutableDictionary<string, object?> stageDict = this.StageData.GetValueOrDefault(stage, ImmutableDictionary<string, object?>.Empty);
        ImmutableDictionary<string, object?> updatedStageDict = stageDict.SetItem(key, value);
        ImmutableDictionary<PipelineStage, ImmutableDictionary<string, object?>> updatedStageData = this.StageData.SetItem(stage, updatedStageDict);

        return this with { StageData = updatedStageData };
    }

    /// <summary>
    /// Gets typed data for a specific pipeline stage.
    /// </summary>
    /// <typeparam name="T">The expected data type.</typeparam>
    /// <param name="stage">The pipeline stage.</param>
    /// <param name="key">The data key.</param>
    /// <returns>The typed data or default value if not found.</returns>
    public T? GetStageData<T>(PipelineStage stage, string key)
    {
        if (!this.StageData.TryGetValue(stage, out ImmutableDictionary<string, object?>? stageDict))
        {
            return default;
        }

        if (!stageDict.TryGetValue(key, out object? value))
        {
            return default;
        }

        if (value is T typedValue)
        {
            return typedValue;
        }

        return default;
    }
}