// <copyright file="DirectoryPathExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Spectre.IO;
using Path = Spectre.IO.Path;

namespace Vellum.Cli.Abstractions.Extensions;

public static class DirectoryPathExtensions
{
    public static IReadOnlyList<DirectoryPath> ChildrenDirectoriesPath(this DirectoryPath directoryPath)
    {
        DirectoryInfo directoryInfo = new(directoryPath.FullPath);
        DirectoryInfo[] directoriesInfos = directoryInfo.GetDirectories();
        var childrenDirectoriesPath = new List<DirectoryPath>();

        foreach (DirectoryInfo childDirectoryInfo in directoriesInfos)
        {
            childrenDirectoriesPath.Add(new DirectoryPath(childDirectoryInfo.FullName));
        }

        return childrenDirectoriesPath.AsReadOnly();
    }

    public static DirectoryPath GetChildDirectoryWithName(this DirectoryPath directoryPath, string directoryName)
    {
        return directoryPath.ToString() + System.IO.Path.DirectorySeparatorChar + directoryName;
    }

    public static IReadOnlyList<FilePath> ChildrenFilesPath(this DirectoryPath directoryPath)
    {
        DirectoryInfo directoryInfo = new(directoryPath.FullPath);
        FileInfo[] filesInfos = directoryInfo.GetFiles();
        List<FilePath> childrenFilesPath = [];

        foreach (FileInfo fileInfo in filesInfos)
        {
            childrenFilesPath.Add(fileInfo.FullName);
        }

        return childrenFilesPath;
    }
}