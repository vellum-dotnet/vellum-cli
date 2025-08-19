using Spectre.IO;

namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Represents processed content after transformation.
/// </summary>
public record ProcessedContent
{
    /// <summary>
    /// Gets the source file path.
    /// </summary>
    public required FilePath SourcePath { get; init; }
    
    /// <summary>
    /// Gets the output file path.
    /// </summary>
    public required FilePath OutputPath { get; init; }
    
    /// <summary>
    /// Gets the transformed content.
    /// </summary>
    public required string Content { get; init; }
    
    /// <summary>
    /// Gets the content type (e.g., "text/html", "application/json").
    /// </summary>
    public string ContentType { get; init; } = "text/html";
    
    /// <summary>
    /// Gets metadata associated with the processed content.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Metadata { get; init; } = new Dictionary<string, object?>();
}