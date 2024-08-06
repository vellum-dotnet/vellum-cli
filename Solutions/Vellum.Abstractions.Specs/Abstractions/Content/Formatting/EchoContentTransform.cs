namespace Vellum.Abstractions.Specs.Abstractions.Content.Formatting;

using Vellum.Abstractions.Content.Formatting;

public class EchoContentTransform : IContentTransform
{
    public string Apply(string input)
    {
        return input;
    }
}