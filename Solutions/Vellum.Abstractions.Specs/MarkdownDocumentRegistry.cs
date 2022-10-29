namespace Vellum.Abstractions.Specs;

using System;
using System.Collections.Generic;
using System.IO;

public class MarkdownDocumentRegistry
{
    private readonly Dictionary<string, string> documents = new();

    public void Register(string documentName, string documentFileName)
    {
        ArgumentNullException.ThrowIfNull(documentName);
        ArgumentNullException.ThrowIfNull(documentFileName);

        this.documents.Add(documentName, documentFileName);
    }

    public Stream GetTemplateStream(string documentName)
    {
        if (!this.documents.TryGetValue(documentName, out string templateFileName))
        {
            throw new InvalidOperationException($"Template not registered: {documentName}");
        }

        return File.OpenRead(Path.Combine(GetRepoRoot(), "Vellum.Abstractions.Specs", "MarkdownDocuments", templateFileName));
    }

    private static string GetRepoRoot()
    {
        string directory = AppContext.BaseDirectory;

        while (!Directory.Exists(Path.Combine(directory, ".git")) && directory != string.Empty)
        {
            directory = Directory.GetParent(directory)?.FullName;
        }

        return directory;
    }
}