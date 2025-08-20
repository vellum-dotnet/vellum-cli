using System.Collections.Immutable;

namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Builder for configuring the Vellum pipeline.
/// </summary>
public class VellumBuilder : IVellumBuilder
{
    private readonly Dictionary<PipelineStage, List<Func<RequestDelegate, RequestDelegate>>> stageMiddleware = new();

    /// <summary>
    /// Adds middleware to a specific pipeline stage.
    /// </summary>
    /// <param name="stage">The pipeline stage to add middleware to.</param>
    /// <param name="middleware">The middleware factory function.</param>
    /// <returns>The builder for fluent configuration.</returns>
    public IVellumBuilder UseInStage(PipelineStage stage, Func<RequestDelegate, RequestDelegate> middleware)
    {
        if (!this.stageMiddleware.ContainsKey(stage))
        {
            this.stageMiddleware[stage] = [];
        }

        this.stageMiddleware[stage].Add(middleware);
        return this;
    }

    /// <summary>
    /// Registers a plugin with the pipeline.
    /// </summary>
    /// <param name="plugin">The plugin to register.</param>
    /// <returns>The builder for fluent configuration.</returns>
    public IVellumBuilder UsePlugin(IVellumPlugin plugin)
    {
        plugin.Configure(this);
        return this;
    }

    /// <summary>
    /// Builds the configured pipeline.
    /// </summary>
    /// <returns>The built pipeline ready for execution.</returns>
    public IVellumPipeline Build()
    {
        // Create an immutable copy of the middleware configuration
        ImmutableDictionary<PipelineStage, ImmutableList<Func<RequestDelegate, RequestDelegate>>> immutableMiddleware =
            this.stageMiddleware.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToImmutableList()
            );

        return new VellumPipeline(immutableMiddleware);
    }
}