namespace Vellum.Abstractions.Specs;

using System;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Reqnroll.BoDi;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Extensions;
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
        services.AddScoped<IExtensionTypeFactory, ExtensionTypeFactory>();
        services.AddScoped<IExtensionDynamicProxyTypeFactory, ExtensionDynamicProxyTypeFactory>();
        services.AddScoped<IContentTypeInterfaceFactory, ContentTypeInterfaceFactory>();
        services.AddWellKnownContentFragmentTypeFactories();
        services.AddWellKnownConverterFactories();
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        this.objectContainer.RegisterInstanceAs(serviceProvider, typeof(IServiceProvider));
    }
}