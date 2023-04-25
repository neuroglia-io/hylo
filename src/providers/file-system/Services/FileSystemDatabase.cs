using Hylo.Infrastructure.Services;
using Hylo.Resources;
using Hylo.Resources.Definitions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Hylo.Providers.FileSystem.Services;

/// <summary>
/// Represents the file system implementation of an Hylo resource database
/// </summary>
public class FileSystemDatabase
    : BackgroundService, IDatabase
{

    /// <summary>
    /// Gets the <see cref="FileSystemDatabase"/>'s connection string, that is the path to the root storage directory
    /// </summary>
    public const string ConnectionStringName = "FileSystem";
    /// <summary>
    /// Gets the <see cref="FileSystemDatabase"/>'s default connection string
    /// </summary>
    public static string DefaultConnectionString { get; } = Path.Combine(AppContext.BaseDirectory, "data");

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="FileSystemDatabase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="configuration">The current <see cref="IConfiguration"/></param>
    /// <param name="cache">The <see cref="IMemoryCache"/> used to cache file last write timestamps in order to throttle observed events to avoid duplicates</param>
    public FileSystemDatabase(ILoggerFactory loggerFactory, IConfiguration configuration, IMemoryCache cache)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.ConnectionString = configuration.GetConnectionString(ConnectionStringName) ?? DefaultConnectionString;
        this.FileSystemWatcher = new();
        this.Cache = cache;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="FileSystemDatabase"/>'s connection string
    /// </summary>
    protected string ConnectionString { get; }

    /// <summary>
    /// Gets the service used to watch the repository's file system
    /// </summary>
    protected FileSystemWatcher FileSystemWatcher { get; }

    /// <summary>
    /// Gets the <see cref="Subject{T}"/> used to observe <see cref="IResource"/> watch events
    /// </summary>
    protected Subject<IResourceWatchEvent> ResourceWatchEvents { get; } = new();

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> used to maintain a plural/kind map of all known resource definitions
    /// </summary>
    protected ConcurrentDictionary<string, string> PluralKindMap { get; } = new();

    /// <summary>
    /// Gets the <see cref="IMemoryCache"/> used to cache file last write timestamps in order to throttle observed events to avoid duplicates
    /// </summary>
    protected IMemoryCache Cache { get; }

    /// <summary>
    /// Gets the <see cref="FileSystemDatabase"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource? CancellationTokenSource { get; private set; }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        var directory = new DirectoryInfo(Path.Combine(this.ConnectionString, FileSystem.ResourceDefinitionsDirectory));
        if (!directory.Exists) directory.Create();

        directory = new DirectoryInfo(Path.Combine(this.ConnectionString, FileSystem.ClusterResourcesDirectory));
        if (!directory.Exists) directory.Create();

        directory = new DirectoryInfo(this.ResolveResourcePath(Namespace.ResourceGroup, Namespace.ResourceVersion, Namespace.ResourcePlural));
        if (!directory.Exists) directory.Create();

        directory = new DirectoryInfo(Path.Combine(this.ConnectionString, FileSystem.NamespacedResourcesDirectory));
        if (!directory.Exists) directory.Create();

        this.PluralKindMap.TryAdd($"{ResourceDefinition.ResourcePlural}.{ResourceDefinition.ResourceGroup}", ResourceDefinition.ResourceKind);
        await foreach (var definition in this.GetDefinitionsAsync(cancellationToken: stoppingToken))
        {
            this.MapDefinitionKind(definition);
        }

        if ((await this.GetDefinitionAsync<Namespace>(this.CancellationTokenSource.Token).ConfigureAwait(false)) == null)
        {
            await this.CreateResourceAsync(new NamespaceDefinition(), false, this.CancellationTokenSource.Token).ConfigureAwait(false);
            await this.CreateNamespaceAsync(Namespace.DefaultNamespaceName, false, this.CancellationTokenSource.Token).ConfigureAwait(false);
            await this.CreateResourceAsync(MutatingWebhook.ResourceDefinition, false, this.CancellationTokenSource.Token).ConfigureAwait(false);
            await this.CreateResourceAsync(ValidatingWebhook.ResourceDefinition, false, this.CancellationTokenSource.Token).ConfigureAwait(false);
        }

        this.FileSystemWatcher.Path = Path.Combine(this.ConnectionString, FileSystem.ResourcesDirectory);
        this.FileSystemWatcher.Filter = "*.json";
        this.FileSystemWatcher.IncludeSubdirectories = true;
        this.FileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
        this.FileSystemWatcher.Created += this.OnFileSystemWatcherEvent;
        this.FileSystemWatcher.Changed += this.OnFileSystemWatcherEvent;
        this.FileSystemWatcher.Deleted += this.OnFileSystemWatcherEvent;
        this.FileSystemWatcher.EnableRaisingEvents = true;
    }

    /// <inheritdoc/>
    public virtual Task<IResource> CreateResourceAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if(string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if(string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        return this.WriteResourceToFileAsync(resource, group, version, plural, @namespace, null, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IResource> GetResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        var directory = new DirectoryInfo(this.ResolveResourcePath(group, version, plural, @namespace));
        var kind = this.ResolveResourceKind(group, plural);

        if (!directory.Exists) yield break;

        foreach (var file in directory.GetFiles("*.json", SearchOption.AllDirectories))
        {
            var resource = await this.ReadResourceFromFileAsync(file, cancellationToken).ConfigureAwait(false);
            if (labelSelectors?.Any() == true && !labelSelectors.All(l => l.Selects(resource))) continue;
            yield return resource;
        }
    }

    /// <inheritdoc/>
    public virtual Task<IResource?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var path = this.ResolveResourcePath(group, version, plural, @namespace, name);
        var file = new FileInfo(path);
        if (file.Exists) return this.ReadResourceFromFileAsync(file, cancellationToken)!;
        else return Task.FromResult(null as IResource);
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection> ListResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        var directory = new DirectoryInfo(this.ResolveResourcePath(group, version, plural, @namespace));
        var kind = this.ResolveResourceKind(group, plural);
        var collection = new Collection(ApiVersion.Build(group, version), kind, new() { }, Array.Empty<object>());

        if (!directory.Exists) return collection;

        foreach (var file in directory.GetFiles("*.json", SearchOption.AllDirectories))
        {
            var resource = await this.ReadResourceFromFileAsync(file, cancellationToken).ConfigureAwait(false);
            if (labelSelectors?.Any() == true)
            {
                var unboxedResource = resource.ConvertTo<Resource>()!;
                if (!labelSelectors.All(l => l.Selects(unboxedResource))) continue;
            }
            collection.Items!.Add(resource);
        }

        return collection;
    }

    /// <inheritdoc/>
    public virtual Task<IResourceWatch> WatchResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        return Task.FromResult<IResourceWatch>(new ResourceWatch(this.ResourceWatchEvents.Where(r => r.Resource.GetGroup() == group && r.Resource.GetVersion() == version && r.Resource.GetNamespace() == @namespace), false));
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchResourceAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceRef = new ResourceReference(new(group, version, plural), name, @namespace);
        var resource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceRef));
        var patchedResource = patch.ApplyTo(resource.ConvertTo<Resource>())!;

        var diffPatch = JsonPatchHelper.CreateJsonPatchFromDiff(resource, patchedResource);
        if (!diffPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceRef));

        return await this.WriteResourceToFileAsync(patchedResource, group, version, plural, @namespace, null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceResourceAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceRef = new ResourceReference(new(group, version, plural), name, @namespace);
        var originalState = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceRef));
        var diffPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalState, resource);
        if (!diffPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceRef));

        return await this.WriteResourceToFileAsync(resource, group, version, plural, @namespace, null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceRef = new ResourceReference(new(group, version, plural), name, @namespace);
        var resource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceRef));
        var patchedResource = patch.ApplyTo(resource.ConvertTo<Resource>())!;

        var diffPatch = JsonPatchHelper.CreateJsonPatchFromDiff(resource, patchedResource);
        if (!diffPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceRef));

        return await this.WriteResourceToFileAsync(patchedResource, group, version, plural, @namespace, subResource, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceRef = new ResourceReference(new(group, version, plural), name, @namespace);
        var originalState = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceRef));
        var diffPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalState, resource);
        if (!diffPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceRef));

        return await this.WriteResourceToFileAsync(resource, group, version, plural, @namespace, subResource, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> DeleteResourceAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException($"Failed to find the specified resource '{group}/{version}/{name}'"); //todo: replace with specialized exception
        File.Delete(this.ResolveResourcePath(group, version, plural, @namespace, name)); //todo: try until file is not locked anymore
        return resource;
    }

    /// <summary>
    /// Resolves the path for the specified resource or resource definition
    /// </summary>
    /// <param name="group">The group the resource to resolve the file path for belongs to</param>
    /// <param name="version">The version of the definition of the resource to resolve the file path for belongs to</param>
    /// <param name="plural">The plural name of the definition of the resource to resolve the file path for belongs to</param>
    /// <param name="namespace">The namespace the resource to resolve the file path for belongs to, if any</param>
    /// <param name="name">The name of the resource to resolve, or null if resolving the path of a resource definition</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected virtual string ResolveResourcePath(string group, string version, string plural, string? @namespace = null, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        string path;
        if (ResourceDefinition.ResourceGroup == group && ResourceDefinition.ResourcePlural == plural)
        {
            path = Path.Combine(this.ConnectionString, FileSystem.ResourceDefinitionsDirectory);
            if (string.IsNullOrWhiteSpace(name)) return path;
            var resourceDefinitionPlural = name.Split('.')[0];
            var resourceDefinitionGroup = name[(resourceDefinitionPlural.Length + 1)..];
            path = Path.Combine(path, resourceDefinitionGroup);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return Path.Combine(path, $"{resourceDefinitionPlural}.json");
        }
        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            path = Path.Combine(this.ConnectionString, FileSystem.NamespacedResourcesDirectory, @namespace);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        else path = Path.Combine(this.ConnectionString, FileSystem.ClusterResourcesDirectory);
        path = Path.Combine(path, group, version, plural);
        if (!string.IsNullOrWhiteSpace(name)) path = Path.Combine(path, $"{name}.json");
        return path;
    }

    /// <summary>
    /// Resolves the kind of the specified resource
    /// </summary>
    /// <param name="group">The API group the resource to resolve the kind of belongs to</param>
    /// <param name="plural">The plural name of the resource to resolve the kind of belongs to</param>
    /// <returns>The plural name of the specified resource</returns>
    protected virtual string ResolveResourceKind(string group, string plural)
    {
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        if (!this.PluralKindMap.TryGetValue($"{plural}.{group}", out var kind) || string.IsNullOrWhiteSpace(kind)) throw new NullReferenceException(); //todo: urgent: throw specifialized exception

        return kind;
    }

    /// <summary>
    /// Adds the specified <see cref="ResourceDefinition"/> to the memory based plural to kind map
    /// </summary>
    /// <param name="resourceDefinition">The <see cref="ResourceDefinition"/> to add</param>
    protected virtual void MapDefinitionKind(IResourceDefinition resourceDefinition)
    {
        if (resourceDefinition == null) throw new ArgumentNullException(nameof(resourceDefinition));
        this.PluralKindMap.TryAdd($"{resourceDefinition.Spec.Names.Plural}.{resourceDefinition.Spec.Group}", resourceDefinition.Spec.Names.Kind);
    }

    /// <summary>
    /// Serializes the specified <see cref="IResource"/> to a file
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to serialize</param>
    /// <param name="group">The API group the <see cref="IResource"/> to serialize belongs to</param>
    /// <param name="version">The version of the <see cref="IResource"/> to serialize</param>
    /// <param name="plural">The plural name of the <see cref="IResource"/> to serialize</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to serialize belongs to</param>
    /// <param name="subResource">The sub resource to serialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The serialized <see cref="IResource"/></returns>
    public virtual async Task<IResource> WriteResourceToFileAsync(IResource resource, string group, string version, string plural, string? @namespace = null, string? subResource = null, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        if (resource.IsResourceDefinition()) this.MapDefinitionKind(resource.ConvertTo<IResourceDefinition>()!);

        var path = this.ResolveResourcePath(group, version, plural, resource.GetNamespace(), resource.GetName());
        var file = new FileInfo(path);
        if (!file.Directory!.Exists) file.Directory!.Create();

        var resourceNode = Serializer.Json.SerializeToNode<object>(resource)!.AsObject();
        resourceNode.Remove(nameof(IMetadata.Metadata).ToCamelCase());
        resource.Metadata.ResourceVersion = string.Format("{0:X}", Serializer.Json.Serialize(resourceNode).GetHashCode());

        if (string.IsNullOrWhiteSpace(subResource)) resource.Metadata.Generation++;

        using var stream = await file.OpenWriteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        stream.SetLength(0);
        await Serializer.Json.SerializeAsync(stream, resource, typeof(object), true, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);

        return resource;
    }

    /// <summary>
    /// Deserializes a <see cref="IResource"/> from the specified file
    /// </summary>
    /// <param name="file">The file to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deserialized resource</returns>
    protected virtual async Task<IResource> ReadResourceFromFileAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        using var stream = await file.OpenReadAsync(cancellationToken).ConfigureAwait(false);
        using var streamReader = new StreamReader(stream);
        var json = await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        return Serializer.Json.Deserialize<Resource>(json)!;
    }

    /// <summary>
    /// Handles the specified <see cref="FileSystemEventArgs"/>
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> to handle</param>
    protected virtual async void OnFileSystemWatcherEvent(object? sender, FileSystemEventArgs e)
    {
        try
        {
            var file = new FileInfo(e.FullPath);
            if (!file.Exists) return;
            if (this.Cache.TryGetValue<DateTime>(file.FullName, out var lastWriteTime)) return;
            this.Cache.Set(file.FullName, file.LastWriteTimeUtc, TimeSpan.FromMilliseconds(5));
            var resource = await this.ReadResourceFromFileAsync(file, this.CancellationTokenSource!.Token);
            this.ResourceWatchEvents.OnNext(new ResourceWatchEvent(e.ChangeType.ToResourceWatchEventType(), resource.ConvertTo<Resource>()!));
        }
        catch (Exception ex)
        {
            this.Logger.LogError("An error occured while handling a file system event concerning file '{fileName}': {ex}", e.FullPath, ex);
        }
    }

    /// <summary>
    /// Disposes of the <see cref="FileSystemDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="FileSystemDatabase"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return ValueTask.CompletedTask;
        this.FileSystemWatcher.EnableRaisingEvents = false;
        this.FileSystemWatcher.Created -= this.OnFileSystemWatcherEvent;
        this.FileSystemWatcher.Changed -= this.OnFileSystemWatcherEvent;
        this.FileSystemWatcher.Dispose();
        this.ResourceWatchEvents.Dispose();
        this.CancellationTokenSource?.Dispose();
        this._disposed = true;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="FileSystemDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="FileSystemDatabase"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.FileSystemWatcher.EnableRaisingEvents = false;
        this.FileSystemWatcher.Created -= this.OnFileSystemWatcherEvent;
        this.FileSystemWatcher.Changed -= this.OnFileSystemWatcherEvent;
        this.FileSystemWatcher.Dispose();
        this.ResourceWatchEvents.Dispose();
        this.CancellationTokenSource?.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Exposes constants about the file system structure used by the <see cref="FileSystemDatabase"/>
    /// </summary>
    public static class FileSystem
    {

        /// <summary>
        /// Gets the name of the directory that contains resources
        /// </summary>
        public const string ResourcesDirectory = "resources";

        /// <summary>
        /// Gets the name of the directory that contains resource definitions
        /// </summary>
        public static readonly string ResourceDefinitionsDirectory = Path.Combine(ResourcesDirectory, "definitions");

        /// <summary>
        /// Gets the name of the directory that contains clustered resources
        /// </summary>
        public static readonly string ClusterResourcesDirectory = Path.Combine(ResourcesDirectory, "cluster");

        /// <summary>
        /// Gets the name of the directory that contains namespaced resources
        /// </summary>
        public static readonly string NamespacedResourcesDirectory = Path.Combine(ResourcesDirectory, "namespaced");

    }

}