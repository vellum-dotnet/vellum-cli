// <copyright file="NuGetTemplatePackageManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Templates
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Markdig;
    using Markdig.Extensions.Yaml;
    using Markdig.Syntax;
    using NDepend.Path;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Packaging.Signing;
    using NuGet.Protocol.Core.Types;
    using NuGet.Resolver;
    using Vellum.Cli.Abstractions.Environment;
    using Vellum.Cli.Abstractions.Templates;
    using YamlDotNet.Serialization;

    public class NuGetTemplatePackageManager : ITemplatePackageManager
    {
        private readonly IAppEnvironment appEnvironment;

        public NuGetTemplatePackageManager(IAppEnvironment appEnvironment)
        {
            this.appEnvironment = appEnvironment;
        }

        public async Task<TemplatePackage> InstallLatestAsync(string packageId)
        {
            TemplatePackage template = await this.GetLatestTemplatePackage(packageId, "any", this.appEnvironment.TemplatesPath).ConfigureAwait(false);

            await this.EnrichTemplatePackage(template).ConfigureAwait(false);

            return template;
        }

        public Task UnnstallAsync(TemplatePackage templatePackage)
        {
            Directory.Delete(templatePackage.TemplateRepositoryPath, recursive: true);

            return Task.CompletedTask;
        }

        private async Task EnrichTemplatePackage(TemplatePackage templatePackage)
        {
            IDeserializer deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

            foreach (Template template in templatePackage.Templates)
            {
                string filePath = Path.GetFullPath(Path.Combine(templatePackage.InstalltionPath, template.NestedFilePath));
                string content = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);

                MarkdownDocument doc = Markdown.Parse(content, pipeline);
                YamlFrontMatterBlock yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

                if (yamlBlock != null)
                {
                    string yaml = string.Join(Environment.NewLine, yamlBlock.Lines.Lines.Select(stringLine => stringLine.ToString()).Where(value => !string.IsNullOrEmpty(value)));
                    Dictionary<string, dynamic> frontMatter = deserializer.Deserialize<Dictionary<string, dynamic>>(yaml);

                    if (frontMatter.TryGetValue("ContentType", out dynamic contentType))
                    {
                        template.ContentType = contentType;
                    }
                }
            }
        }

        private async Task<TemplatePackage> GetLatestTemplatePackage(string packageId, string frameworkVersion, IAbsoluteDirectoryPath templateRepositoryPath)
        {
            ISettings settings = Settings.LoadSpecificSettings(root: null, this.appEnvironment.NuGetConfigFilePath.ToString());

            var nugetFramework = NuGetFramework.ParseFolder(frameworkVersion);
            var sourceRepositoryProvider = new SourceRepositoryProvider(new PackageSourceProvider(settings), Repository.Provider.GetCoreV3());

            using (var cacheContext = new SourceCacheContext())
            {
                IEnumerable<SourceRepository> repositories = sourceRepositoryProvider.GetRepositories();
                var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);

                foreach (SourceRepository sourceRepository in repositories)
                {
                    DependencyInfoResource dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>().ConfigureAwait(false);

                    IEnumerable<SourcePackageDependencyInfo> dependencyInfo = await dependencyInfoResource.ResolvePackages(
                        packageId,
                        nugetFramework,
                        cacheContext,
                        NullLogger.Instance,
                        CancellationToken.None).ConfigureAwait(false);

                    if (dependencyInfo == null)
                    {
                        continue;
                    }

                    availablePackages.AddRange(dependencyInfo);
                }

                var resolverContext = new PackageResolverContext(
                    DependencyBehavior.Highest,
                    new[] { packageId },
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<PackageReference>(),
                    Enumerable.Empty<PackageIdentity>(),
                    availablePackages,
                    sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                    NullLogger.Instance);

                var resolver = new PackageResolver();

                SourcePackageDependencyInfo packageToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
                                                                       .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)))
                                                                       .FirstOrDefault();

                var packagePathResolver = new PackagePathResolver(SettingsUtility.GetGlobalPackagesFolder(settings));

                var packageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(settings, NullLogger.Instance),
                    NullLogger.Instance);

                string installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                PackageReaderBase packageReader;

                if (installedPath == null && packageToInstall != null)
                {
                    DownloadResource downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None).ConfigureAwait(false);

                    DownloadResourceResult downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                        packageToInstall,
                        new PackageDownloadContext(cacheContext),
                        SettingsUtility.GetGlobalPackagesFolder(settings),
                        NullLogger.Instance,
                        CancellationToken.None).ConfigureAwait(false);

                    await PackageExtractor.ExtractPackageAsync(
                        downloadResult.PackageSource,
                        downloadResult.PackageStream,
                        packagePathResolver,
                        packageExtractionContext,
                        CancellationToken.None).ConfigureAwait(false);

                    packageReader = downloadResult.PackageReader;
                }
                else
                {
                    packageReader = new PackageFolderReader(installedPath);
                }

                PackageIdentity identity = await packageReader.GetIdentityAsync(CancellationToken.None).ConfigureAwait(false);

                var templatePackageMetaData = new TemplatePackage
                {
                    PackageId = identity.Id,
                    Version = identity.Version.OriginalVersion,
                    TemplateRepositoryPath = templateRepositoryPath.ToString(),
                };

                foreach (FrameworkSpecificGroup contentItem in packageReader.GetContentItems())
                {
                    foreach (string item in contentItem.Items)
                    {
                        templatePackageMetaData.Templates.Add(new Template { NestedFilePath = item });
                    }
                }

                var packageFileExtractor = new PackageFileExtractor(
                    templatePackageMetaData.Templates.Select(template => template.NestedFilePath),
                    XmlDocFileSaveMode.None);

                await packageReader.CopyFilesAsync(
                    templatePackageMetaData.InstalltionPath,
                    templatePackageMetaData.Templates.Select(template => template.NestedFilePath),
                    packageFileExtractor.ExtractPackageFile,
                    NullLogger.Instance,
                    CancellationToken.None).ConfigureAwait(false);

                return templatePackageMetaData;
            }
        }
    }
}