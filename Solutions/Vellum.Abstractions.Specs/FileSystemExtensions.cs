
using System.IO;

namespace Vellum.Abstractions.Specs;
public static class FileSystemExtensions
{
    public static string NormaliseCrossPlatformDirectorySeparators(this string ambiguousPath)
    {
        return ambiguousPath.Replace(@"\", "/").Replace('/', Path.DirectorySeparatorChar);
    }
}