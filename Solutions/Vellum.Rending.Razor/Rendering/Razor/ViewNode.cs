using System.Diagnostics;
using Spectre.IO;

namespace Vellum.Cli.Rendering.Razor;

[DebuggerDisplay("{FilePath.FullPath}, Children = {Children.Count}")]
public record ViewNode(FilePath FilePath, List<ViewNode> Children);