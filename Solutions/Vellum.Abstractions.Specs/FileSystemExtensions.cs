namespace Vellum.Abstractions.Specs;

using System.IO;

public static class FileSystemExtensions
{
    public static string NormaliseCrossPlatformDirectorySeparators(this string ambiguousPath)
    {
        return ambiguousPath.Replace(@"\", "/").Replace('/', Path.DirectorySeparatorChar);
    }
}