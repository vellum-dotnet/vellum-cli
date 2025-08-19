namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Represents the Vellum execution pipeline.
/// </summary>
public interface IVellumPipeline
{
    /// <summary>
    /// Executes the pipeline with the given context.
    /// </summary>
    /// <param name="context">The initial context for the pipeline.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    ValueTask ExecuteAsync(VellumContext context);
}