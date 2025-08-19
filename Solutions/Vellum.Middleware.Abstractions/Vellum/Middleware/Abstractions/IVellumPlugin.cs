namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Defines a plugin that can extend the Vellum pipeline.
/// </summary>
public interface IVellumPlugin
{
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    string Version { get; }
    
    /// <summary>
    /// Gets the description of the plugin.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Configures the plugin with the Vellum builder.
    /// </summary>
    /// <param name="builder">The Vellum builder to configure.</param>
    void Configure(IVellumBuilder builder);
}