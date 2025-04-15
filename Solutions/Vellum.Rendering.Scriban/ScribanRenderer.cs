using Scriban;
using Spectre.IO;

namespace Vellum.Rendering.Scriban;

public class ScribanRenderer
{
    public async ValueTask<FilePath> RenderAsync(FilePath renderedFilePath, string templateContent, dynamic model)
    {
        Template template = Template.ParseLiquid(templateContent);
        string result = await template.RenderAsync(model).ConfigureAwait(false);

        return await this.WriteFileAsync(renderedFilePath, result);
    }

    private async ValueTask<FilePath> WriteFileAsync(FilePath fileName, string content)
    {
        if (!FileSystem.Shared.Directory.Exists(fileName.GetDirectory()))
        {
            FileSystem.Shared.Directory.Create(fileName.GetDirectory());
        }

        await File.WriteAllTextAsync(fileName.FullPath, content).ConfigureAwait(false);

        return fileName;
    }
}