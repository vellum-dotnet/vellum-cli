// <copyright file="ContentHashing.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Vellum.Abstractions.Caching;

public static class ContentHashing
{
    public static string Hash(string content)
    {
        using var sha256Managed = SHA256.Create();
        byte[] textData = Encoding.UTF8.GetBytes(content);
        byte[] hash = sha256Managed.ComputeHash(textData);

        return string.Concat(sha256Managed.Hash!.Select(x => x.ToString("x2")));
    }
}