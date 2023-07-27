using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
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
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    public PluginManager(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

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
        this.Logger.LogDebug("Scanning for plugin files in '{pluginsDirectory}'...", this.PluginsDirectory.FullName);
        if (!this.PluginsDirectory.Exists) this.PluginsDirectory.Create();
        var assemblyFiles = this.PluginsDirectory.GetFiles("*.dll", SearchOption.AllDirectories).ToList();
        var files = this.PluginsDirectory.GetFiles("*.plugin.json", SearchOption.AllDirectories).ToList();
        files.AddRange(this.PluginsDirectory.GetFiles("plugin.json", SearchOption.AllDirectories));
        this.Logger.LogDebug("{pluginCount} plugin metadata files have been found", files.Count);
        foreach (var pluginFile in files)
        {
            var json = await File.ReadAllTextAsync(pluginFile.FullName, this.CancellationTokenSource.Token).ConfigureAwait(false);
            var pluginMetadata = JsonSerializer.Deserialize<PluginMetadata>(json)!;
            if (!string.IsNullOrWhiteSpace(pluginMetadata.NugetPackage)) await this.InstallPluginPackageAsync(pluginFile, pluginMetadata, this.CancellationTokenSource.Token).ConfigureAwait(false);
            var assemblyFilePath = pluginMetadata.AssemblyFilePath;
            if (!Path.IsPathRooted(assemblyFilePath)) assemblyFilePath = Path.GetFullPath(assemblyFilePath, pluginFile.Directory!.FullName);
            var assemblyFile = new FileInfo(assemblyFilePath);
            if (!assemblyFile.Exists) throw new FileNotFoundException($"Failed to find the specified plugin assembly '{assemblyFilePath}'");
            if (!assemblyFiles.Any(f => f.FullName == assemblyFile.FullName)) assemblyFiles.Add(assemblyFile);
        }
        foreach(var assemblyFile in this.PluginsDirectory.GetFiles("*.dll", SearchOption.AllDirectories).ToList())
        {
            if (!assemblyFiles.Any(f => f.FullName == assemblyFile.FullName)) assemblyFiles.Add(assemblyFile);
        }
        var runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
        var defaultAssemblies = AssemblyLoadContext.Default.Assemblies.Select(a => a.Location).Except(runtimeAssemblies);
        var appAssemblies = new FileInfo(typeof(PluginManager).Assembly.Location).Directory!.GetFiles("*.dll").Select(f => f.FullName).Except(runtimeAssemblies).Except(defaultAssemblies);
        foreach (var assemblyFile in assemblyFiles)
        {
            var assemblies = new List<string>(runtimeAssemblies) { assemblyFile.FullName };
            assemblies.AddRange(defaultAssemblies);
            assemblies.AddRange(appAssemblies);
            var resolver = new PluginPathAssemblyResolver(assemblies.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct(), assemblyFiles.Select(f => f.FullName));
            using var metadataContext = new MetadataLoadContext(resolver);
            var assembly = metadataContext.LoadFromAssemblyPath(assemblyFile.FullName);
            foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && !t.IsGenericType))
            {
                var pluginAttribute = type.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.FullName == typeof(PluginAttribute).FullName);
                if (pluginAttribute == null) continue;
                var pluginMetadata = PluginMetadata.FromType(type);
                if (!Path.IsPathRooted(pluginMetadata.AssemblyFilePath)) pluginMetadata.AssemblyFilePath = Path.GetFullPath(pluginMetadata.AssemblyFilePath, assemblyFile.DirectoryName!);
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
    /// <param name="metadataFile">The plugin file</param>
    /// <param name="metadata">The metadata of the plugin based on the Nuget package to download and extract</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask InstallPluginPackageAsync(FileInfo metadataFile, PluginMetadata metadata, CancellationToken cancellationToken)
    {
        if (metadataFile == null) throw new ArgumentNullException(nameof(metadataFile));
        if (metadata == null) throw new ArgumentNullException(nameof(metadata));
        if(string.IsNullOrWhiteSpace(metadata.NugetPackage)) throw new ArgumentNullException(nameof(metadata.NugetPackage));

        this.Logger.LogDebug("Checking plugin package file...");

        var components = metadata.NugetPackage.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var packageSource = components.Length == 1 ? "https://api.nuget.org/v3/index.json" : components.First();
        var repository = NuGet.Protocol.Core.Types.Repository.Factory.GetCoreV3(packageSource);
        var cache = new SourceCacheContext();
        var downloadMap = new Dictionary<string, Version>();

        var packageId = components.Last();
        components = packageId.Split(':', StringSplitOptions.RemoveEmptyEntries);
        packageId = components[0];
        var packageVersion = (components.Length > 1 ? NuGetVersion.Parse(components[1]) : null) ?? await repository.GetPackageLatestVersionAsync(packageId, cancellationToken).ConfigureAwait(false);
        var package = new PackageIdentity(packageId, packageVersion);

        var packageMetadataFile = new FileInfo(Path.Combine(metadataFile.Directory!.FullName, $"{Path.GetFileNameWithoutExtension(metadataFile.Name)}.package.json"));
        PluginPackageMetadata? packageMetadata = null;
        if (packageMetadataFile.Exists) packageMetadata = Serializer.Json.Deserialize<PluginPackageMetadata>(await File.ReadAllTextAsync(packageMetadataFile.FullName, cancellationToken).ConfigureAwait(false));
        if (packageMetadata != null && packageMetadata.Identity.Id == packageId && new NuGetVersion(packageMetadata.Identity.Version) >= packageVersion)
        {
            this.Logger.LogDebug("Plugin package is already installed and up to date");
            return;
        }

        this.Logger.LogDebug("Checking plugin package dependencies...");
        var packageDependencies = await repository.ListPackageDependenciesAsync(package, cache, true, cancellationToken).ConfigureAwait(false);

        foreach(var dependency in packageDependencies)
        {
            var dependencyIdentity = new NuGetPackageIdentity() { Id = dependency.Id, Version = (dependency.VersionRange.HasUpperBound ? dependency.VersionRange.MaxVersion : dependency.VersionRange.MinVersion).OriginalVersion };
            if(packageMetadata?.Dependencies.Any(d => d.Id == dependencyIdentity.Id && new NuGetVersion(d.Version) >= new NuGetVersion(dependencyIdentity.Version)) == true)
            {
                this.Logger.LogDebug("Package dependency '{dependency}' is already installed and up to date", dependencyIdentity);
                continue;
            }
            this.Logger.LogDebug("Installing package dependency '{dependency}'...", dependencyIdentity);
            await repository.DownloadAndExtractPackageAsync(new(dependencyIdentity.Id, new(dependencyIdentity.Version)), metadataFile.Directory!, cache, cancellationToken).ConfigureAwait(false);
            this.Logger.LogDebug("Package dependency '{dependency}' has been successfully installed", dependencyIdentity);
        }
        await repository.DownloadAndExtractPackageAsync(package, metadataFile.Directory!, cache, cancellationToken).ConfigureAwait(false);

        packageMetadata = new PluginPackageMetadata(new() { Id = package.Id, Version = package.Version.OriginalVersion }, packageDependencies.Select(d => new NuGetPackageIdentity() { Id = d.Id, Version = (d.VersionRange.HasUpperBound ? d.VersionRange.MaxVersion : d.VersionRange.MinVersion).OriginalVersion }));
        await File.WriteAllTextAsync(packageMetadataFile.FullName, Serializer.Json.Serialize(packageMetadata), cancellationToken).ConfigureAwait(false);

        this.Logger.LogDebug("Plugin package successfully installed");
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

}