using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Vellum.Abstractions.Content.Converters;

namespace Vellum.Abstractions.Specs;

[TestClass]
public class FaqConverterTests
{
    private readonly FaqConverter converter = new();

    [TestMethod]
    public void Keys_are_matched_by_name_rather_than_position()
    {
        Dictionary<object, object> entry = new()
        {
            ["Answer"] = "42",
            ["Question"] = "What is the answer?",
        };

        (string question, string answer) = this.converter.Convert(entry);

        question.ShouldBe("What is the answer?");
        answer.ShouldBe("42");
    }

    [TestMethod]
    public void Keys_are_matched_case_insensitively()
    {
        Dictionary<object, object> entry = new()
        {
            ["question"] = "What is the answer?",
            ["answer"] = "42",
        };

        (string question, string answer) = this.converter.Convert(entry);

        question.ShouldBe("What is the answer?");
        answer.ShouldBe("42");
    }

    [TestMethod]
    public void Short_form_q_and_a_keys_are_accepted()
    {
        Dictionary<object, object> entry = new()
        {
            ["Q"] = "What is the answer?",
            ["A"] = "42",
        };

        (string question, string answer) = this.converter.Convert(entry);

        question.ShouldBe("What is the answer?");
        answer.ShouldBe("42");
    }

    [TestMethod]
    public void A_missing_key_fails_with_a_descriptive_exception()
    {
        Dictionary<object, object> entry = new()
        {
            ["Question"] = "What is the answer?",
        };

        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => this.converter.Convert(entry));

        exception.Message.ShouldContain("Answer");
    }
}