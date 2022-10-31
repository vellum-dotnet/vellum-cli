﻿namespace Vellum.Abstractions.Specs;

using System;

using BoDi;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

using Vellum.Abstractions.Content.Formatting;
using Vellum.Abstractions.Specs.Abstractions.Content.Formatting;

[Binding]
public class ContainerSetup
{
    private readonly IObjectContainer objectContainer;

    public ContainerSetup(IObjectContainer objectContainer)
    {
        this.objectContainer = objectContainer;
    }

    [BeforeScenario()]
    public void Initialize()
    {
        ServiceCollection services = new();

        services.AddScoped<IContentFormatter, ContentFormatter>();
        services.AddScoped<IContentTransform, EchoContentTransform>();
        services.AddScoped<IContentTransform, EmptyContentTransform>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        this.objectContainer.RegisterInstanceAs(serviceProvider, typeof(IServiceProvider));
    }
}