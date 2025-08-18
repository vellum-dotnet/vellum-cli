using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Vellum.Cli.Rendering.Razor;

public class ViewRender : IViewRender
{
    private readonly IRazorViewEngine viewEngine;
    private readonly ITempDataProvider tempDataProvider;
    private readonly IServiceProvider serviceProvider;

    public ViewRender(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        this.viewEngine = viewEngine;
        this.tempDataProvider = tempDataProvider;
        this.serviceProvider = serviceProvider;
    }

    public async Task<string> Render<TModel>(string name, TModel model) //where TModel : RenderBase<TContent> where TContent : ContentBase
    {
        ActionContext actionContext = GetActionContext();
        ViewEngineResult viewEngineResult = viewEngine.FindView(actionContext, name, false);

        if (!viewEngineResult.Success)
        {
            // TODO If we can't find the "WarmUp" view, we should create it and retry...
            throw new InvalidOperationException($"Couldn't find view '{name}'");
        }

        IView view = viewEngineResult.View;

        // TODO: investigate object creation 
        await using StringWriter output = new();
        ViewContext viewContext = new(
            actionContext,
            view,
            new ViewDataDictionary<TModel>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new())
            {
                Model = model,
            },
            new TempDataDictionary(
                actionContext.HttpContext,
                tempDataProvider),
            output,
            new());

        await view.RenderAsync(viewContext).ConfigureAwait(false);

        return output.ToString();
    }

    private ActionContext GetActionContext()
    {
        DefaultHttpContext httpContext = new()
        {
            RequestServices = serviceProvider
        };

        return new(httpContext, new(), new());
    }
}