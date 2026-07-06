
using System;
using System.IO;

namespace Vellum.Abstractions.Specs;

public static class TestDocuments
{
    public static FileInfo GetMarkdownFile(string fileName)
    {
        return new FileInfo(Path.Combine(GetSpecsRoot(), "MarkdownDocuments", fileName));
    }

    public static string GetHtmlContent(string fileName)
    {
        return File.ReadAllText(Path.Combine(GetSpecsRoot(), "HtmlDocuments", fileName));
    }

    private static string GetSpecsRoot()
    {
        string? directory = AppContext.BaseDirectory;

        while (directory is not null && !Directory.Exists(Path.Combine(directory, ".git")))
        {
            directory = Directory.GetParent(directory)?.FullName;
        }

        if (directory is null)
        {
            throw new InvalidOperationException($"Could not locate the repository root (a directory containing .git) walking up from {AppContext.BaseDirectory}");
        }

        return Path.Combine(directory, "Solutions", "Vellum.Abstractions.Specs");
    }
}