using Vellum.Abstractions.Content;

namespace Vellum.Abstractions.Specs;

/// <summary>
/// A test-only target interface exercising recursive duck-typing: a fragment-valued metadata member
/// (Author) mapped onto another interface (IAuthor).
/// </summary>
public interface IAuthoredContent
{
    IAuthor Author { get; set; }
}