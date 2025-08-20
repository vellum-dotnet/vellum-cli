using Spectre.IO;

namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Represents a content file discovered in the pipeline.
/// </summary>
public record ContentFile
{
    /// <summary>
    /// Gets the file path.
    /// </summary>
    public required FilePath Path { get; init; }

    /// <summary>
    /// Gets the raw content of the file.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets metadata associated with the file.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Metadata { get; init; } = new Dictionary<string, object?>();
}