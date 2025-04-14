// <copyright file="FaqConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Vellum.Abstractions.Content.Converters;

public class FaqConverter : IConverter<Dictionary<object, object>>
{
    public object Convert(Dictionary<object, object> item)
    {
        (string Question, string Answer) faq = default;

        if (item.ElementAt(0).Value is string question && item.ElementAt(1).Value is string answer)
        {
            faq = (question, answer);
        }

        return faq;
    }
}