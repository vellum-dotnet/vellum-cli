using Microsoft.Extensions.DependencyInjection;
using Vellum.Abstractions.Rendering;

namespace Vellum.Cli.Rendering.Razor;

public class RenderingApi : IRenderingApi
{
    private readonly IServiceProvider serviceProvider;

    public RenderingApi(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<string> RenderContent<TModel>(TModel model, string name)
    {
        using IServiceScope scope = this.serviceProvider.CreateScope();
        IViewRender viewRender = scope.ServiceProvider.GetRequiredService<IViewRender>();
        return await viewRender.Render(name, model).ConfigureAwait(false);
    }
}