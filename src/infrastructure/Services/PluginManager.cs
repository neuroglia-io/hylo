using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IPluginManager"/> interface
/// </summary>
public class PluginManager
    : IHostedService, IPluginManager
{

    /// <summary>
    /// Initializes a new <see cref="PluginManager"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public PluginManager(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the <see cref="DirectoryInfo"/> to scan for plugins
    /// </summary>
    protected DirectoryInfo PluginsDirectory { get; } = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "plugins"));

    /// <summary>
    /// Gets a <see cref="ConcurrentBag{T}"/>
    /// </summary>
    protected ConcurrentBag<PluginMetadata> AvailablePlugins { get; } = new();

    /// <summary>
    /// Gets the <see cref="PluginManager"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <inheritdoc/>
    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (!this.PluginsDirectory.Exists) this.PluginsDirectory.Create();
        var assemblyFiles = this.PluginsDirectory.GetFiles("*.dll", SearchOption.AllDirectories).ToList();
        foreach (var pluginFile in this.PluginsDirectory.GetFiles("*.plugin.json", SearchOption.AllDirectories))
        {
            var json = await File.ReadAllTextAsync(pluginFile.FullName, this.CancellationTokenSource.Token).ConfigureAwait(false);
            var pluginMetadata = JsonSerializer.Deserialize<PluginMetadata>(json)!;
            if (!string.IsNullOrWhiteSpace(pluginMetadata.NugetPackage)) await this.DownloadAndExtractNugetPackageAsync(pluginMetadata, this.CancellationTokenSource.Token).ConfigureAwait(false);
            var assemblyFilePath = pluginMetadata.AssemblyFilePath;
            if (pluginMetadata.AssemblyFilePath.StartsWith(Path.DirectorySeparatorChar)) assemblyFilePath = Path.Combine(pluginFile.Directory!.FullName, assemblyFilePath);
            var assemblyFile = new FileInfo(assemblyFilePath);
            if (!assemblyFile.Exists) throw new FileNotFoundException($"Failed to find the specified plugin assembly '{assemblyFilePath}'");
            assemblyFiles.Add(assemblyFile);
        }
        foreach (var assemblyFile in assemblyFiles)
        {
            var runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            var defaultAssemblies = AssemblyLoadContext.Default.Assemblies.Select(a => a.Location).Except(runtimeAssemblies);
            var appAssemblies = new FileInfo(typeof(PluginManager).Assembly.Location).Directory!.GetFiles("*.dll").Select(f => f.FullName).Except(runtimeAssemblies).Except(defaultAssemblies);
            var assemblies = new List<string>(runtimeAssemblies) { assemblyFile.FullName };
            assemblies.AddRange(defaultAssemblies);
            assemblies.AddRange(appAssemblies);
            var resolver = new PathAssemblyResolver(assemblies.Where(a => !string.IsNullOrWhiteSpace(a)));
            using var metadataContext = new MetadataLoadContext(resolver);
            var assembly = metadataContext.LoadFromAssemblyPath(assemblyFile.FullName);
            foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && !t.IsGenericType))
            {
                var pluginAttribute = type.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.FullName == typeof(PluginAttribute).FullName);
                if (pluginAttribute == null) continue;
                var pluginMetadata = PluginMetadata.FromType(type);
                this.AvailablePlugins.Add(pluginMetadata);
            }
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<TContract> FindPluginsAsync<TContract>([EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TContract : class
    {
        foreach (var metadata in this.AvailablePlugins.Where(m => m.ContractTypeName == typeof(TContract).FullName!).ToList())
        {
            var plugin = new Plugin(this.ServiceProvider, metadata);
            yield return await plugin.LoadAsync<TContract>(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Downloads and extracts the specified Nuget package
    /// </summary>
    /// <param name="metadata">The metadata of the plugin based on the Nuget package to download and extract</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DownloadAndExtractNugetPackageAsync(PluginMetadata metadata, CancellationToken cancellationToken)
    {
        if (metadata == null) throw new ArgumentNullException(nameof(metadata));
        if(string.IsNullOrWhiteSpace(metadata.NugetPackage)) throw new ArgumentNullException(nameof(metadata.NugetPackage));
        var components = metadata.NugetPackage.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var packageSource = components.Length == 1 ? "https://api.nuget.org/v3/index.json" : components.First();
        var packageId = components.Last();

        components = packageId.Split(':', StringSplitOptions.RemoveEmptyEntries);
        packageId = components[0];
        var packageVersion = components.Length > 1 ? components[1] : null;
        var repository = NuGet.Protocol.Core.Types.Repository.Factory.GetCoreV3(packageSource);
        var cache = new SourceCacheContext();
        var packageSearchResource = await repository.GetResourceAsync<PackageSearchResource>().ConfigureAwait(false);
        var findPackageByIdResource = await repository.GetResourceAsync<FindPackageByIdResource>().ConfigureAwait(false);
        var searchFilter = new SearchFilter(includePrerelease: true);
        var results = await packageSearchResource.SearchAsync(packageId, searchFilter, 0, 100, NuGet.Common.NullLogger.Instance, cancellationToken).ConfigureAwait(false);
        var result = results.FirstOrDefault() ?? throw new NullReferenceException($"Failed to find nuget package with id '{packageId}' in source '{packageSource}'");
        var versions = await result.GetVersionsAsync().ConfigureAwait(false);
        var version = string.IsNullOrWhiteSpace(packageVersion) ? versions.OrderByDescending(v => v.Version.Version).First() : versions.FirstOrDefault(v => v.Version.Version.ToString() == packageVersion) ?? throw new NullReferenceException($"Failed to find version '{packageVersion}' of nuget package '{packageId}' in source '{packageSource}'");
        var packageFileName = Path.Combine(this.PluginsDirectory.FullName, $"{result.Identity.Id}.{version.Version}.nupkg");
        var packageOutputDirectory = Path.Combine(new FileInfo(metadata.AssemblyFilePath).Directory!.FullName);
        Stream packageStream;
        if (File.Exists(packageFileName))
        {
            packageStream = File.OpenRead(packageFileName);
        }
        else
        {
            packageStream = File.Open(packageFileName, FileMode.Create);
            await findPackageByIdResource.CopyNupkgToStreamAsync(result.Identity.Id, version.Version, packageStream, cache, NuGet.Common.NullLogger.Instance, cancellationToken).ConfigureAwait(false);
            await packageStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            packageStream.Position = 0;
        }
        using var packageReader = new PackageArchiveReader(packageStream);
        var currentFramework = NuGetFramework.Parse(typeof(PluginManager).Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName);
        var libItems = await packageReader.GetLibItemsAsync(cancellationToken).ConfigureAwait(false);
        var framework = libItems
            .Where(f => DefaultCompatibilityProvider.Instance.IsCompatible(currentFramework, f.TargetFramework))
            .Last();
        foreach (var item in framework.Items)
        {
            var outputFile = Path.Combine(packageOutputDirectory, item.Split('/').Last());
            packageReader.ExtractFile(item, outputFile, NuGet.Common.NullLogger.Instance);
        }
        await packageStream.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

}
