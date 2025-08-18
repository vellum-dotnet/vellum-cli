using System.Text.RegularExpressions;
using Spectre.IO;
using Vellum.Cli.Rendering.Razor;

namespace Endjin.StaticSiteGenerator.Rendering;

public partial class RazorViewGraphBuilder
{
    [GeneratedRegex("""(?:Html\.(?:Render)?Partial(?:Async)?|Component\.InvokeAsync|<partial name=)\(?"(?<name>.*)"\)?""")]
    private static partial Regex PartialRegex();

    [GeneratedRegex("""@\{[\s\S]*Layout[\s\S]*=[\s\S]*"(?<file>.*\.cshtml)"[\s?\S]*;[\s?\S]*\}""")]
    private static partial Regex LayoutRegex();

    private readonly FileSystem fs = new();

    public ViewNode BuildGraph(DirectoryPath directoryPath)
    {
        if (directoryPath.IsRelative)
        {
            directoryPath = directoryPath.MakeAbsolute(Spectre.IO.Environment.Shared.WorkingDirectory);
        }

        IEnumerable<IFile> files = this.fs.GetDirectory(directoryPath).GetFiles("*.cshtml", SearchScope.Recursive);
        ViewNode rootNode = new("Root", []);

        foreach (IFile file in files)
        {
            IEnumerable<ViewNode> nodes = this.ParseRazorFile(directoryPath, file);

            rootNode.Children.AddRange(nodes);
        }

        return rootNode;
    }

    private IEnumerable<ViewNode> ParseRazorFile(DirectoryPath directoryPath, IFile file)
    {
        string content = file.ReadAllText();
        MatchCollection partials = PartialRegex().Matches(content);
        MatchCollection layouts = LayoutRegex().Matches(content);
        List<ViewNode> children = [];

        foreach (Match layout in layouts)
        {
            string layoutName = layout.Groups["file"].Value.TrimStart('~').TrimStart('/').Replace('/', '\\');
            FilePath? layoutPath = this.ResolveViewLocation(directoryPath, layoutName);

            if (layoutPath is not null)
            {
                IFile layoutFile = this.fs.GetFile(layoutPath);

                children.AddRange(this.ParseRazorFile(directoryPath, layoutFile));
            }
        }

        foreach (Match match in partials)
        {
            string partialName = match.Groups["name"].Value.TrimStart('~').TrimStart('/').Replace('/', '\\');
            FilePath? childPath = this.ResolveViewLocation(directoryPath, partialName);

            if (childPath is not null)
            {
                IFile childFile = fs.GetFile(childPath);

                children.AddRange(this.ParseRazorFile(directoryPath, childFile));
            }
        }

        yield return new(file.Path, children);
    }

    private FilePath? ResolveViewLocation(DirectoryPath rootDirectory, string partialName)
    {
        FilePath partialFile = new FilePath(partialName).ChangeExtension(".cshtml");

        FilePath[] possibleLocations =
        [
            rootDirectory.CombineWithFilePath(partialFile),
            rootDirectory.Combine("shared").CombineWithFilePath(partialFile),
            rootDirectory.Combine("Shared").CombineWithFilePath(partialFile),
        ];

        foreach (FilePath location in possibleLocations)
        {
            if (this.fs.Exist(location))
            {
                return location;
            }
        }

        return null; // Partial view not found
    }
}