﻿// <copyright file="NuGetPluginPackageManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Packages
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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
    using Vellum.Cli.Abstractions.Plugins;
    using Vellum.Cli.Abstractions.Templates;

    public class NuGetPluginPackageManager
    {
        private readonly IAppEnvironment appEnvironment;

        public NuGetPluginPackageManager(IAppEnvironment appEnvironment)
        {
            this.appEnvironment = appEnvironment;
        }

        public Task UninstallAsync(string packageId)
        {
            var pluginPath = Path.Join(this.appEnvironment.PluginPath.ToString(), packageId);

            if (Directory.Exists(pluginPath))
            {
                Directory.Delete(pluginPath, recursive: true);
            }

            return Task.CompletedTask;
        }

        public async Task<PluginPackage> InstallLatestAsync(string packageId)
        {
            var packageMetaData = await this.GetLatestTemplatePackage(packageId, "any", this.appEnvironment.PluginPath).ConfigureAwait(false);

            return packageMetaData;
        }

        private async Task<PluginPackage> GetLatestTemplatePackage(string packageId, string frameworkVersion, IAbsoluteDirectoryPath pluginRepositoryPath)
        {
            var nugetFramework = NuGetFramework.ParseFolder(frameworkVersion);
            var settings = Settings.LoadSpecificSettings(root: null, this.appEnvironment.NuGetConfigFilePath.ToString());
            var sourceRepositoryProvider = new SourceRepositoryProvider(new PackageSourceProvider(settings), Repository.Provider.GetCoreV3());

            var packageMetaDataList = new List<PluginPackage>();

            using (var cacheContext = new SourceCacheContext())
            {
                var repositories = sourceRepositoryProvider.GetRepositories();
                var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);

                foreach (var sourceRepository in repositories)
                {
                    var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>().ConfigureAwait(false);

                    var dependencyInfo = await dependencyInfoResource.ResolvePackages(
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

                try
                {
                    SourcePackageDependencyInfo packageToInstall = resolver
                        .Resolve(resolverContext, CancellationToken.None).Select(p =>
                            availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)))
                        .FirstOrDefault();

                    var packagePathResolver = new PackagePathResolver(SettingsUtility.GetGlobalPackagesFolder(settings));

                    var packageExtractionContext = new PackageExtractionContext(
                        PackageSaveMode.Defaultv3,
                        XmlDocFileSaveMode.None,
                        ClientPolicyContext.GetClientPolicy(settings, NullLogger.Instance),
                        NullLogger.Instance);

                    var frameworkReducer = new FrameworkReducer();
                    string installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                    PackageReaderBase packageReader;

                    if (installedPath == null && packageToInstall != null)
                    {
                        DownloadResource downloadResource = await packageToInstall.Source
                            .GetResourceAsync<DownloadResource>(CancellationToken.None).ConfigureAwait(false);

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

                    var packageMetaData = new PluginPackage
                    {
                        Name = identity.Id,
                        Version = identity.Version.OriginalVersion,
                        RepositoryPath = pluginRepositoryPath,
                    };

                    foreach (var contentItem in await packageReader.GetContentItemsAsync(CancellationToken.None).ConfigureAwait(false))
                    {
                        packageMetaData.Plugins.AddRange(contentItem.Items);
                    }

                    var packageFileExtractor = new PackageFileExtractor(
                        packageMetaData.Plugins,
                        XmlDocFileSaveMode.None);

                    await packageReader.CopyFilesAsync(
                        packageMetaData.PluginPath.ToString(),
                        packageMetaData.Plugins,
                        packageFileExtractor.ExtractPackageFile,
                        NullLogger.Instance,
                        CancellationToken.None).ConfigureAwait(false);

                    foreach (IAbsoluteFilePath file in packageMetaData.PluginPath.GetChildDirectoryWithName("content").ChildrenFilesPath)
                    {
                        File.Copy(file.ToString(), Path.Join(packageMetaData.PluginPath.ToString(), file.FileName), overwrite: true);
                    }

                    packageMetaData.Plugins.Clear();

                    foreach (IAbsoluteFilePath file in packageMetaData.PluginPath.ChildrenFilesPath)
                    {
                        packageMetaData.Plugins.Add(file.GetRelativePathFrom(packageMetaData.PluginPath).ToString());
                    }

                    Directory.Delete(packageMetaData.PluginPath.GetChildDirectoryWithName("content").ToString(), recursive: true);

                    packageMetaDataList.Add(packageMetaData);
                }
                catch (NuGetResolverConstraintException exception)
                {
                    var foo = exception.Message;
                }
            }

            return packageMetaDataList.FirstOrDefault();
        }
    }
}
