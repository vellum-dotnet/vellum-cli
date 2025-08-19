namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Represents a function that processes a VellumContext through the pipeline.
/// </summary>
/// <param name="context">The pipeline context to process.</param>
/// <returns>A ValueTask representing the asynchronous operation.</returns>
public delegate ValueTask RequestDelegate(VellumContext context);