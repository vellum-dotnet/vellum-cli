﻿using Reqnroll;

namespace Vellum.Cli.Specs;

using System;

[Binding]
public class CommandLineParserSteps
{
    [Given(@"I have entered (.*) into the calculator")]
    public void GivenIHaveEnteredIntoTheCalculator(int p0)
    {
    }

    [When(@"I press add")]
    public void WhenIPressAdd()
    {
    }

    [Then(@"the result should be (.*) on the screen")]
    public void ThenTheResultShouldBeOnTheScreen(int p0)
    {
    }
}