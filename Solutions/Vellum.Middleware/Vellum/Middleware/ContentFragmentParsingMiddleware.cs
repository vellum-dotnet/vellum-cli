using Vellum.Middleware.Abstractions;

namespace Vellum.Middleware;

public class ContentFragmentParsingMiddleware
{
    /// <summary>
    /// Initializes a new instance of the ContentFragmentParsingMiddleware class.
    /// </summary>
    public ContentFragmentParsingMiddleware()
    {
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


            VellumContext updatedContext = builder.Build();

            await next(updatedContext);
        };
    }
}