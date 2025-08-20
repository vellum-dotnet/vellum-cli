using System.Collections.Immutable;

namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Implementation of the Vellum execution pipeline with optimizations.
/// </summary>
public class VellumPipeline : IVellumPipeline
{
    private readonly ImmutableDictionary<PipelineStage, ImmutableList<Func<RequestDelegate, RequestDelegate>>> stageMiddleware;
    private readonly Dictionary<PipelineStage, RequestDelegate> cachedPipelines = new();
    private readonly PipelineStage[] stagesWithMiddleware;

    /// <summary>
    /// Initializes a new instance of the VellumPipeline class.
    /// </summary>
    /// <param name="stageMiddleware">The middleware configuration for each stage.</param>
    public VellumPipeline(ImmutableDictionary<PipelineStage, ImmutableList<Func<RequestDelegate, RequestDelegate>>> stageMiddleware)
    {
        this.stageMiddleware = stageMiddleware;
        // Pre-compute stages that have middleware for efficient skipping
        this.stagesWithMiddleware = stageMiddleware.Keys.OrderBy(s => (int)s).ToArray();

        // Pre-build all pipelines for caching
        foreach (KeyValuePair<PipelineStage, ImmutableList<Func<RequestDelegate, RequestDelegate>>> kvp in stageMiddleware)
        {
            // Build the pipeline chain once with a dummy terminal
            RequestDelegate pipeline = this.BuildStagePipeline(kvp.Value);
            this.cachedPipelines[kvp.Key] = pipeline;
        }
    }

    /// <summary>
    /// Executes the pipeline with the given context.
    /// </summary>
    /// <param name="context">The initial context for the pipeline.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    public async ValueTask ExecuteAsync(VellumContext context)
    {
        // Skip empty stages - only execute stages with middleware
        if (this.stagesWithMiddleware.Length == 0)
        {
            // No middleware at all, nothing to do
            return;
        }

        // Thread context through stages
        VellumContext currentContext = context;

        // Only execute stages that have middleware
        foreach (PipelineStage stage in this.stagesWithMiddleware)
        {
            // Update context with current stage
            currentContext = currentContext with { CurrentStage = stage };

            // Get the middleware list for this stage
            ImmutableList<Func<RequestDelegate, RequestDelegate>> middlewareList = this.stageMiddleware[stage];

            // Create a container to capture the final context
            ContextContainer container = new(currentContext);

            // Create a terminal that captures the context
            ValueTask Terminal(VellumContext ctx)
            {
                container.Context = ctx;
                return ValueTask.CompletedTask;
            }

            // Build the pipeline chain for this execution
            // This ensures proper context threading
            RequestDelegate pipeline = Terminal;
            foreach (Func<RequestDelegate, RequestDelegate> middleware in middlewareList.Reverse())
            {
                pipeline = middleware(pipeline);
            }

            // Execute the pipeline with the current context
            await pipeline(currentContext);

            // Update the current context for the next stage
            currentContext = container.Context;
        }
    }

    private RequestDelegate BuildStagePipeline(ImmutableList<Func<RequestDelegate, RequestDelegate>> middlewareList)
    {
        // Terminal delegate - does nothing
        RequestDelegate pipeline = async ctx => await ValueTask.CompletedTask;

        // Build the pipeline by chaining middleware in reverse order
        // This ensures the first middleware runs first
        foreach (Func<RequestDelegate, RequestDelegate> middleware in middlewareList.Reverse())
        {
            pipeline = middleware(pipeline);
        }

        return pipeline;
    }
}