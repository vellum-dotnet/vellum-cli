using Spectre.IO;

namespace Vellum.Cli.Rendering.Razor;

public static class ViewNodeExtensions
{
    public static IEnumerable<ViewNode> FindAllViewNodesRecursivelyByFilePath(this ViewNode root, FilePath file)
    {
        foreach (ViewNode child in root.Children)
        {
            // Use the helper method to check if this child or any of its descendants match
            if (TraverseAndCheck(child, file))
            {
                // If a match is found within this child's subtree, return the child
                yield return child;
            }
        }
    }

    private static bool TraverseAndCheck(ViewNode node, FilePath file)
    {
        if (PathComparer.Default.Compare(node.FilePath, file) == 0)
        {
            return true;
        }

        // Recursively check each child
        foreach (ViewNode child in node.Children)
        {
            if (TraverseAndCheck(child, file))
            {
                return true;
            }
        }

        // No match found in this node or its descendants
        return false;
    }
}