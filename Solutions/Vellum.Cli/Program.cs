// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli
{
    using System;
    using System.Threading.Tasks;

    public static class Program
    {
        public static Task<int> Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            return Task.FromResult(0);
        }
    }
}