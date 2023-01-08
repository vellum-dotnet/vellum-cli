namespace Vellum.Abstractions.Specs;

using System;

public static class SpecExtensions
{
    /// <summary>
    /// Normalize line endings to the current platform.
    /// </summary>
    /// <param name="content">Text to normalize.</param>
    /// <returns>Lines normalized to the current platforms line endings.</returns>
    public static string NormalizeLineEndings(this string content)
    {
        ArgumentException.ThrowIfNullOrEmpty(content);

        return content.Replace(PlatformLineEndings.WindowsLineEnding, PlatformLineEndings.UnixLineEnding)
                      .Replace(PlatformLineEndings.MacOsLineEnding, PlatformLineEndings.UnixLineEnding)
                      .Replace(PlatformLineEndings.UnixLineEnding, PlatformLineEndings.WindowsLineEnding);
    }
}