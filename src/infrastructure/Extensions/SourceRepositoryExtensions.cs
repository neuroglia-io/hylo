using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Reflection;
using System.Runtime.Versioning;

namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="SourceRepository"/> instances
/// </summary>
public static class SourceRepositoryExtensions
{

    static readonly NuGetFramework CurrentFramework = NuGetFramework.Parse(typeof(PluginManager).Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName);

    /// <summary>
    /// Downloads and extracts the specified nuget package and all its dependencies
    /// </summary>
    /// <param name="repository">The extended <see cref="SourceRepository"/></param>
    /// <param name="id">The id of the package to download</param>
    /// <param name="version">The version of the package to download, if any. If null, the latest package version will be retrieved</param>
    /// <param name="outputDirectory">The directory to download and extract the package to</param>
    /// <param name="cache">The <see cref="SourceCacheContext"/> to use</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    public static async ValueTask DownloadAndExtractPackageAsync(this SourceRepository repository, string id, NuGetVersion? version, DirectoryInfo outputDirectory, SourceCacheContext cache, CancellationToken cancellationToken = default)
    {
        version ??= await repository.GetPackageLatestVersionAsync(id, cancellationToken).ConfigureAwait(false);
        var packageFileName = Path.Combine(outputDirectory.FullName, $"{id}.{version.Version}.nupkg");
        Stream packageStream;
        if (File.Exists(packageFileName))
        {
            packageStream = File.OpenRead(packageFileName);
        }
        else
        {
            packageStream = File.Open(packageFileName, FileMode.Create);
            var findPackageById = await repository.GetResourceAsync<FindPackageByIdResource>().ConfigureAwait(false);
            await findPackageById.CopyNupkgToStreamAsync(id, version, packageStream, cache, NuGet.Common.NullLogger.Instance, cancellationToken).ConfigureAwait(false);
            await packageStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            packageStream.Position = 0;
        }
        using var packageReader = new PackageArchiveReader(packageStream);
        var dependencies = await packageReader.GetPackageDependenciesAsync(cancellationToken).ConfigureAwait(false);
        foreach (var dependency in dependencies.Where(f => DefaultCompatibilityProvider.Instance.IsCompatible(CurrentFramework, f.TargetFramework)).SelectMany(dg => dg.Packages).Distinct().Where(p => !p.Id.StartsWith("System.")))
        {
            var targetVersion = dependency.VersionRange.HasUpperBound ? dependency.VersionRange.MaxVersion : dependency.VersionRange.HasLowerBound ? dependency.VersionRange.MinVersion : null;
            await repository.DownloadAndExtractPackageAsync(dependency.Id, targetVersion, outputDirectory, cache, cancellationToken).ConfigureAwait(false);
        }
        var libItems = await packageReader.GetLibItemsAsync(cancellationToken).ConfigureAwait(false);
        var framework = libItems.Where(f => DefaultCompatibilityProvider.Instance.IsCompatible(CurrentFramework, f.TargetFramework)).LastOrDefault();
        if (framework != null)
        {
            foreach (var item in framework.Items)
            {
                var outputFile = Path.Combine(outputDirectory.FullName, item.Split('/').Last());
                packageReader.ExtractFile(item, outputFile, NuGet.Common.NullLogger.Instance);
            }
        }
        await packageStream.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the latest version of the specified Nuget package
    /// </summary>
    /// <param name="repository">The extended <see cref="SourceRepository"/></param>
    /// <param name="id">The id of the package to get the latest version of</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The latest version of the specified Nuget package</returns>
    /// <exception cref="NullReferenceException"></exception>
    public static async ValueTask<NuGetVersion> GetPackageLatestVersionAsync(this SourceRepository repository, string id, CancellationToken cancellationToken = default)
    {
        var search = await repository.GetResourceAsync<PackageSearchResource>().ConfigureAwait(false);
        var searchFilter = new SearchFilter(false);
        var searchResults = await search.SearchAsync(id, searchFilter, 0, 100, NuGet.Common.NullLogger.Instance, cancellationToken).ConfigureAwait(false);
        var searchResult = searchResults.FirstOrDefault() ?? throw new NullReferenceException($"Failed to find nuget package with id '{id}' in source '{repository.PackageSource.SourceUri}'");
        var versions = await searchResult.GetVersionsAsync().ConfigureAwait(false);
        var versionInfo = versions.OrderByDescending(v => v.Version.Version).First();
        return new(versionInfo.Version);
    }

}