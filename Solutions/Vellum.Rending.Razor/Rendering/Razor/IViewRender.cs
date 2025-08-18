namespace Vellum.Cli.Rendering.Razor;

public interface IViewRender
{
    Task<string> Render<TModel>(string name, TModel model);
}