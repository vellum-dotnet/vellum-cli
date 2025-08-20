using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Vellum.Cli.Rendering.Razor;

public class Startup
{
    public Startup(IWebHostEnvironment env)
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddEnvironmentVariables();

        // We must provide a default value for the application name, otherwise Razor will not look for dep.json files
        // and then will not pick up the latest language version and default to C# 8.0
        env.ApplicationName = "vellum";

        Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "MyPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
        });

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        services.AddRouting();
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseRouting();
        app.UseDefaultFiles();
        app.UseResponseCompression();
        app.UseServerSideExtensionsChangeNotifications("/sse/connect");

        // Custom Middleware or Routing Logic to rewrite paths
        app.Use(async (context, next) =>
        {
            string? path = context.Request.Path.Value;

            // Check if the path does not have an extension
            if (path is not null && !System.IO.Path.HasExtension(path) && !path.StartsWith("/api"))
            {
                // Rewrite the path to include .html extension
                context.Request.Path = $"{path}.html";
                // You must also set the PathBase to empty, otherwise, the static file middleware won't be able to find the file.
                context.Request.PathBase = "";
            }

            context.Response.Headers[HeaderNames.CacheControl] = "no-store"; // "no-cache";

            await next();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Needs to be after custom routing logic
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = new FileExtensionContentTypeProvider
            {
                Mappings =
                {
                    [".scss"] = "text/x-scs",
                    [".svg"] = "image/svg+xml",
                    [".woff"] = "font/woff",
                    [".woff2"] = "font/woff2",
                    [".gltf"] = "model/gltf+json",
                    [".glb"] = "model/gltf-binary",
                    [".bin"] = "application/octet-stream"
                }
            }
        });
    }
}