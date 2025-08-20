namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Builder interface for configuring the Vellum pipeline.
/// </summary>
public interface IVellumBuilder
{
    /// <summary>
    /// Adds middleware to a specific pipeline stage.
    /// </summary>
    /// <param name="stage">The pipeline stage to add middleware to.</param>
    /// <param name="middleware">The middleware factory function.</param>
    /// <returns>The builder for fluent configuration.</returns>
    IVellumBuilder UseInStage(PipelineStage stage, Func<RequestDelegate, RequestDelegate> middleware);

    /// <summary>
    /// Registers a plugin with the pipeline.
    /// </summary>
    /// <param name="plugin">The plugin to register.</param>
    /// <returns>The builder for fluent configuration.</returns>
    IVellumBuilder UsePlugin(IVellumPlugin plugin);

    /// <summary>
    /// Builds the configured pipeline.
    /// </summary>
    /// <returns>The built pipeline ready for execution.</returns>
    IVellumPipeline Build();
}