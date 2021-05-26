﻿// <copyright file="ContentHashing.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Caching
{
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class ContentHashing
    {
        public static string Hash(string content)
        {
            using (var sha256Managed = new SHA256Managed())
            {
                // Compute hash from text parameter
                sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(content));

                // Return as hexadecimal string
                return string.Join(string.Empty, sha256Managed.Hash.Select(x => x.ToString("x2")));
            }
        }
    }
}