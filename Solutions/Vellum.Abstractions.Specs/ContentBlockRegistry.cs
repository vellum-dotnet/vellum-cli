namespace Vellum.Abstractions.Specs;

using System;
using System.Collections.Generic;
using Vellum.Abstractions.Content;

public class ContentBlockRegistry
{
    private readonly Dictionary<string, ContentBlock> documents = new();

    public void Register(string id, string contentType, string specPath)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(contentType);
        ArgumentNullException.ThrowIfNull(specPath);

        ContentBlock contentBlock = new(new ContentSpecification(null, null, specPath, null), contentType, id);

        this.documents.Add(id, contentBlock);
    }

    public ContentBlock Get(string id)
    {
        if (!this.documents.TryGetValue(id, out ContentBlock contentBlock))
        {
            throw new InvalidOperationException($"ContentBlock not registered: {id}");
        }

        return contentBlock;
    }
}