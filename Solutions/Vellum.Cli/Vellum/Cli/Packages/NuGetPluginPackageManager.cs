// <copyright file="NuGetPluginPackageManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;

using Spectre.Console;
using Spectre.IO;

using Vellum.Cli.Abstractions.Environment;
using Vellum.Cli.Abstractions.Extensions;
using Vellum.Cli.Abstractions.Plugins;

namespace Vellum.Cli.Packages;

public class NuGetPluginPackageManager(IAppEnvironment appEnvironment)
{
    public Task UninstallAsync(string packageId)
    {
        string pluginPath = System.IO.Path.Join(appEnvironment.PluginPath.ToString(), packageId);

        if (Directory.Exists(pluginPath))
        {
            Directory.Delete(pluginPath, recursive: true);
        }

        return Task.CompletedTask;
    }

    public async Task<PluginPackage?> InstallVersionAsync(string packageId, string version)
    {
        PluginPackage? packageMetaData = await this.GetLatestTemplatePackage(packageId, version, "any", appEnvironment.PluginPath).ConfigureAwait(false);

        return packageMetaData;
    }

    public async Task<PluginPackage?> InstallLatestAsync(string packageId)
    {
        PluginPackage? packageMetaData = await this.GetLatestTemplatePackage(packageId, string.Empty, "any", appEnvironment.PluginPath).ConfigureAwait(false);

        return packageMetaData;
    }

    private async Task<PluginPackage?> GetLatestTemplatePackage(string packageId, string version, string frameworkVersion, DirectoryPath pluginRepositoryPath)
    {
        var nugetFramework = NuGetFramework.ParseFolder(frameworkVersion);
        ISettings settings = Settings.LoadSpecificSettings(root: string.Empty, appEnvironment.NuGetConfigFilePath.ToString());
        SourceRepositoryProvider sourceRepositoryProvider = new(new PackageSourceProvider(settings), Repository.Provider.GetCoreV3());

        var packageMetaDataList = new List<PluginPackage>();

        using (SourceCacheContext cacheContext = new())
        {
            IEnumerable<SourceRepository> repositories = sourceRepositoryProvider.GetRepositories();
            HashSet<SourcePackageDependencyInfo> availablePackages = new(PackageIdentityComparer.Default);

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

            DependencyBehavior dependencyBehavior = DependencyBehavior.Highest;
            List<PackageIdentity> packageIdentities = [];
            SourcePackageDependencyInfo? matchingPackage = availablePackages.FirstOrDefault(p => p.Version.OriginalVersion == version);

            if (matchingPackage != null)
            {
                PackageIdentity pi = new(packageId, matchingPackage.Version);
                packageIdentities.Add(pi);

                // Allow preview versions to be resolved.
                dependencyBehavior = DependencyBehavior.Ignore;
            }

            PackageResolverContext resolverContext = new(
                dependencyBehavior,
                new[] { packageId },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                packageIdentities,
                availablePackages,
                sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                NullLogger.Instance);

            var resolver = new PackageResolver();

            try
            {
                IEnumerable<PackageIdentity> matches = resolver.Resolve(resolverContext, CancellationToken.None);

                SourcePackageDependencyInfo? packageToInstall = resolver
                    .Resolve(resolverContext, CancellationToken.None)
                    .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)))
                    .FirstOrDefault();

                PackagePathResolver packagePathResolver = new(SettingsUtility.GetGlobalPackagesFolder(settings));

                var packageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(settings, NullLogger.Instance),
                    NullLogger.Instance);

                FrameworkReducer frameworkReducer = new();
                string installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                PackageReaderBase packageReader;

                if (string.IsNullOrEmpty(installedPath) && packageToInstall != null)
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

                var packageMetaData = new PluginPackage
                {
                    Name = identity.Id,
                    Version = identity.Version.OriginalVersion,
                    RepositoryPath = pluginRepositoryPath,
                };

                foreach (FrameworkSpecificGroup contentItem in await packageReader.GetContentItemsAsync(CancellationToken.None).ConfigureAwait(false))
                {
                    packageMetaData.Plugins.AddRange(contentItem.Items);
                }

                PackageFileExtractor packageFileExtractor = new(
                    packageMetaData.Plugins,
                    XmlDocFileSaveMode.None);

                await packageReader.CopyFilesAsync(
                    packageMetaData.PluginPath.ToString(),
                    packageMetaData.Plugins,
                    packageFileExtractor.ExtractPackageFile,
                    NullLogger.Instance,
                    CancellationToken.None).ConfigureAwait(false);

                try
                {
                    foreach (FilePath file in packageMetaData.PluginPath.GetChildDirectoryWithName("content").ChildrenFilesPath())
                    {
                        File.Copy(file.ToString()!, System.IO.Path.Join(packageMetaData.PluginPath.ToString(), file.GetFilename().ToString()), overwrite: true);
                    }
                }
                catch (IOException exception)
                {
                    throw new InvalidOperationException(
                        "Are you trying to install a .nupkg which doesn't have a payload in its 'content' folder? Have you created a standard .nupkg or a custom 'plugin' using a .nuspec file?",
                        exception);
                }

                packageMetaData.Plugins.Clear();

                foreach (FilePath file in packageMetaData.PluginPath.ChildrenFilesPath())
                {
                    // TODO: Figure out relative path.
                    packageMetaData.Plugins.Add(file.GetRelativePathFrom(packageMetaData.PluginPath).ToString());
                }

                Directory.Delete(packageMetaData.PluginPath.GetChildDirectoryWithName("content").ToString()!, recursive: true);

                packageMetaDataList.Add(packageMetaData);
            }
            catch (NuGetResolverConstraintException exception)
            {
                AnsiConsole.WriteLine(exception.Message);
            }
        }

        return packageMetaDataList.FirstOrDefault();
    }
}