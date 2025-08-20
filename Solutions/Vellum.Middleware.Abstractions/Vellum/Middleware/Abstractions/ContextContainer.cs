namespace Vellum.Middleware.Abstractions;

/// <summary>
/// A mutable container for passing context through the pipeline.
/// </summary>
internal class ContextContainer
{
    /// <summary>
    /// Gets or sets the current context.
    /// </summary>
    public VellumContext Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the ContextContainer class.
    /// </summary>
    /// <param name="context">The initial context.</param>
    public ContextContainer(VellumContext context)
    {
        this.Context = context;
    }
}