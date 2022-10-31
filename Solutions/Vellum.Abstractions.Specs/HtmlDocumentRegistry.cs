namespace Vellum.Abstractions.Specs;

using System;
using System.Collections.Generic;
using System.IO;

public class HtmlDocumentRegistry
{
    private readonly Dictionary<string, string> documents = new();

    public void Register(string documentName, string documentFileName)
    {
        ArgumentNullException.ThrowIfNull(documentName);
        ArgumentNullException.ThrowIfNull(documentFileName);

        this.documents.Add(documentName, documentFileName);
    }

    public FileInfo GetFile(string documentName)
    {
        if (!this.documents.TryGetValue(documentName, out string templateFileName))
        {
            throw new InvalidOperationException($"Template not registered: {documentName}");
        }

        return new FileInfo(Path.Combine(GetRepoRoot(), "Solutions", "Vellum.Abstractions.Specs", "HtmlDocuments", templateFileName));
    }

    public string GetFileContent(string documentName)
    {
        return File.ReadAllText(this.GetFile(documentName).FullName);
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