// <copyright file="ServiceCollectionExtension.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vellum.Abstractions.Rendering;

namespace Vellum.Cli.Rendering.Razor.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureRenderingServices(this IServiceCollection services)
    {
        // We need to set up the webhost to register all the types that the Razor Engine needs.
        services.UseWebHost();
        services.AddScoped<IViewRender, ViewRender>();
        services.AddScoped<IRenderingApi, RenderingApi>();

        services.Configure<RazorViewEngineOptions>(opt =>
        {
            opt.ViewLocationFormats.Add("{0}.cshtml");
            opt.ViewLocationFormats.Add("/views/{1}/{0}" + RazorViewEngine.ViewExtension);
            opt.ViewLocationFormats.Add("/views/shared/{1}" + RazorViewEngine.ViewExtension);
            opt.ViewLocationFormats.Add("/views/shared/{0}" + RazorViewEngine.ViewExtension);
            opt.ViewLocationFormats.Add("/views/shared/{1}/{0}" + RazorViewEngine.ViewExtension);
            opt.ViewLocationFormats.Add("/shared/{1}" + RazorViewEngine.ViewExtension);
            opt.ViewLocationFormats.Add("/shared/{0}" + RazorViewEngine.ViewExtension);
            opt.ViewLocationFormats.Add("/shared/{1}/{0}" + RazorViewEngine.ViewExtension);
        });

        services.AddControllersWithViews();
        services.AddRazorPages(opt =>
            {
                // Without this, the view engine seems not to find anything in
                // the Shared folder, although it's not at all clear why this
                // fixes it. It was cribbed from:
                // https://weblog.west-wind.com/posts/2019/Nov/05/Dynamically-Loading-Assemblies-at-Runtime-in-RazorPages#limited-razor-pages-content
                opt.RootDirectory = "/";
            })
            .AddRazorRuntimeCompilation(opt =>
            {
                LoadPrivateBinAssemblies(opt, AppContext.BaseDirectory); // reference app assemblies
                LoadPrivateBinAssemblies(opt, Path.Combine(AppContext.BaseDirectory, "refs")); //reference all the ref assemblies
                //opt.FileProviders.Add(new PhysicalFileProvider(GlobalSettings.Site.ThemePath.FullPath));
            });

        return services;
    }

    private static void UseWebHost(this IServiceCollection services)
    {
        IHost host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
        {
            builder.UseStartup<Startup>();
            builder.SuppressStatusMessages(true);
            builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
            builder.ConfigureServices(serviceCollection =>
            {
                foreach (ServiceDescriptor service in serviceCollection)
                {
                    // Let's copy all the WebHost services into the main application service
                    // collection, as we need all the types to support razor rendering.
                    services.Add(service);
                }
            });
        }).Build();

        services.AddSingleton(host);
    }

    private static void LoadPrivateBinAssemblies(MvcRazorRuntimeCompilationOptions opt, string path)
    {
        if (Directory.Exists(path))
        {
            foreach (var file in Directory.GetFiles(path).Where(x => x.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)))
            {
                try
                {
                    opt.AdditionalReferencePaths.Add(file);
                }
                catch (Exception)
                {
                    //AnsiConsole.WriteLine(ex.Message);
                }
            }
        }
    }
}