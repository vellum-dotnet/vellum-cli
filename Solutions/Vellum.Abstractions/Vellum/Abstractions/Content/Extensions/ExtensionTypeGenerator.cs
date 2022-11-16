// <copyright file="ExtensionTypeGenerator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content.Extensions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Vellum.Abstractions.Caching;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public static class ExtensionTypeGenerator
{
    public static Type Generate(Type baseType, List<Type> extensionTypes)
    {
        IEnumerable<string> extensionNamespaces = extensionTypes.DistinctBy(x => x.Namespace).Select(x => x.Namespace);
        string extensionNames = string.Join("_", extensionTypes.OrderBy(x => x.FullName).Distinct(x => x.FullName));
        string hash = ContentHashing.Hash(extensionNames);
        string dynamicTypeName = baseType.Name + "_Extensions_" + hash;

        SeparatedSyntaxList<BaseTypeSyntax> baseTypes = SeparatedList<BaseTypeSyntax>();

        baseTypes = baseTypes.Add(SimpleBaseType(ParseTypeName(baseType.Name)));

        foreach (Type extensionType in extensionTypes)
        {
            baseTypes = baseTypes.Add(SimpleBaseType(ParseTypeName(extensionType.Name)));
        }

        InterfaceDeclarationSyntax interfaceBlock = InterfaceDeclaration(dynamicTypeName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithBaseList(
                BaseList(
                    Token(SyntaxKind.ColonToken), baseTypes));

        NamespaceDeclarationSyntax ns = NamespaceDeclaration(ParseName(baseType.Namespace)).AddMembers(interfaceBlock);

        foreach (string extensionNamespace in extensionNamespaces)
        {
            ns = ns.AddUsings(UsingDirective(ParseName(extensionNamespace)));
        }

        CompilationUnitSyntax cu = CompilationUnit().AddMembers(ns);

        PortableExecutableReference mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        PortableExecutableReference blog = MetadataReference.CreateFromFile(baseType.Assembly.Location);
        CSharpCompilationOptions options = new(OutputKind.DynamicallyLinkedLibrary);
        var compilation = CSharpCompilation.Create(assemblyName: hash, syntaxTrees: new[] { cu.SyntaxTree }, references: new[] { mscorlib, blog }, options: options);

        using var ms = new MemoryStream();
        EmitResult emitResult = compilation.Emit(ms);

        var ourAssembly = Assembly.Load(ms.ToArray());
        Type type = ourAssembly.ExportedTypes.FirstOrDefault(x => x.Name == dynamicTypeName);
        return type;
        /*Directory.CreateDirectory(@"c:\code-gen");
        await using var streamWriter = new StreamWriter(@"c:\code-gen\generated.cs", false);
        ns.NormalizeWhitespace().WriteTo(streamWriter);*/
    }
}