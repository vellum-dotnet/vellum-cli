// <copyright file="FaqConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Vellum.Abstractions.Content.Converters;

/// <summary>
/// Converts a frontmatter FAQ entry into a (Question, Answer) tuple. Keys are matched by name
/// (case-insensitively, accepting 'Question'/'Q' and 'Answer'/'A') rather than by position, so the order
/// of the keys in the YAML is irrelevant, and a malformed entry fails loudly instead of yielding an
/// empty tuple.
/// </summary>
public class FaqConverter : IConverter<Dictionary<object, object>, (string Question, string Answer)>
{
    public (string Question, string Answer) Convert(Dictionary<object, object> value)
    {
        return (GetRequiredString(value, "Question", "Q"), GetRequiredString(value, "Answer", "A"));
    }

    private static string GetRequiredString(Dictionary<object, object> entry, string key, string shortKey)
    {
        foreach (KeyValuePair<object, object> pair in entry)
        {
            if (pair.Key is string candidate
                && (string.Equals(candidate, key, StringComparison.OrdinalIgnoreCase) || string.Equals(candidate, shortKey, StringComparison.OrdinalIgnoreCase)))
            {
                return pair.Value?.ToString()
                    ?? throw new InvalidOperationException($"The FAQ entry's '{candidate}' key has a null value. Each entry under 'FAQs' must contain a '{key}' (or '{shortKey}') and an 'Answer' (or 'A').");
            }
        }

        throw new InvalidOperationException($"The FAQ entry is missing a '{key}' (or '{shortKey}') key. Each entry under 'FAQs' must contain a 'Question' (or 'Q') and an 'Answer' (or 'A').");
    }
}