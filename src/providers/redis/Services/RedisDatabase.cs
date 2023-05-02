﻿using Hylo.Infrastructure.Services;
using Hylo.Resources;
using Hylo.Resources.Definitions;
using Json.Patch;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Hylo.Providers.Redis;

/// <summary>
/// Represents a <see href="https://redis.io/">Redis</see> implementation of the <see cref="Infrastructure.Services.IDatabase"/> interface
/// </summary>
public class RedisDatabase
    : Infrastructure.Services.IDatabase
{

    /// <summary>
    /// Gets the name of the <see cref="RedisDatabase"/>'s connection string
    /// </summary>
    public const string ConnectionStringName = "redis";
    const string ClusterResourcePrefix = "cluster.";
    static readonly RedisChannel WatchEventChannel = "watch-events";

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="RedisDatabase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="redis">The current <see cref="IConnectionMultiplexer"/></param>
    public RedisDatabase(ILoggerFactory loggerFactory, IConnectionMultiplexer redis)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Redis = redis;
        this.Database = redis.GetDatabase();
        this.Subscriber = redis.GetSubscriber();
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the current <see cref="IConnectionMultiplexer"/>
    /// </summary>
    protected IConnectionMultiplexer Redis { get; }

    /// <summary>
    /// Gets the current <see cref="Infrastructure.Services.IDatabase"/>
    /// </summary>
    protected StackExchange.Redis.IDatabase Database { get; }

    /// <summary>
    /// Gets the current <see cref="ISubscriber"/>
    /// </summary>
    protected ISubscriber Subscriber { get; }

    /// <summary>
    /// Gets the <see cref="Subject{T}"/> used to observe <see cref="IResource"/> watch events
    /// </summary>
    protected Subject<IResourceWatchEvent> ResourceWatchEvents { get; } = new();

    /// <summary>
    /// Gets the <see cref="RedisDatabase"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <inheritdoc/>
    public virtual async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var initialized = false;
        await this.Subscriber.SubscribeAsync(WatchEventChannel, OnWatchEvent).ConfigureAwait(false);
        if ((await this.GetDefinitionAsync<Namespace>(this.CancellationTokenSource.Token).ConfigureAwait(false)) == null)
        {
            initialized = true;
            await this.CreateResourceAsync(new NamespaceDefinition(), false, this.CancellationTokenSource.Token).ConfigureAwait(false);
        }
        if ((await this.ListResourcesAsync<Namespace>(cancellationToken: this.CancellationTokenSource.Token).ConfigureAwait(false)).Items?.Any() != true)
        {
            initialized = true;
            await this.CreateNamespaceAsync(Namespace.DefaultNamespaceName, false, this.CancellationTokenSource.Token).ConfigureAwait(false);
        }
        return initialized;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> CreateResourceAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return await this.WriteResourceAsync(group, version, plural, resource.ConvertTo<Resource>()!, true, ResourceWatchEventType.Created, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return await this.ReadResourceAsync(this.BuildResourceKey(group, version, plural, name, @namespace), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection> ListResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        var resourceDefinition = await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceDefinitionNotFound(new ResourceDefinitionReference(group, version, plural)));
        var items = await this.GetResourcesAsync(group, version, plural, @namespace, labelSelectors, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);

        return new Collection(ApiVersion.Build(group, version), resourceDefinition.Spec.Names.Kind, new() { }, items);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IResource> GetResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        var resourceDefinition = await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceDefinitionNotFound(new ResourceDefinitionReference(group, version, plural)));
        var definitionIndexKey = this.BuildResourceByDefinitionIndexKey(group, plural);
        var pattern = resourceDefinition.Spec.Scope switch
        {
            ResourceScope.Cluster => $"{ClusterResourcePrefix}*",
            ResourceScope.Namespaced => string.IsNullOrWhiteSpace(@namespace) ? "*" : $"{@namespace}.*",
            _ => throw new NotSupportedException($"The specified {nameof(ResourceScope)} '{EnumHelper.Stringify(resourceDefinition.Spec.Scope)}' is not supported")
        };
        var results = this.Database
            .HashScanAsync(definitionIndexKey, pattern)
            .SelectAwait(async e => (await this.ReadResourceAsync((string)e.Value!, cancellationToken).ConfigureAwait(false))!);
        if (labelSelectors?.Any() == true) results = results.Select(r => r.ConvertTo<Resource>()!).Where(r => labelSelectors.All(s => s.Selects(r)));
        await foreach (var result in results)
        {
            yield return result;
        }
    }

    /// <inheritdoc/>
    public virtual Task<IResourceWatch> WatchResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return Task.FromResult<IResourceWatch>(new ResourceWatch(this.ResourceWatchEvents, false));
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchResourceAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var originalResource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));
        var updatedResource = patch.ApplyTo(originalResource.ConvertTo<Resource>()!)!;

        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalResource, updatedResource);
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => (o.Path.Segments[0] == nameof(IMetadata.Metadata).ToCamelCase() && (o.Path.Segments[1] == nameof(ResourceMetadata.Annotations).ToCamelCase() 
            || o.Path.Segments[1] == nameof(ResourceMetadata.Labels).ToCamelCase()))|| o.Path.Segments.First() == nameof(ISpec.Spec).ToCamelCase()));
        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceReference));

        return await this.WriteResourceAsync(group, version, plural, jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>()!)!, true, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceResourceAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var originalResource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));
        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalResource, resource);
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => (o.Path.Segments[0] == nameof(IMetadata.Metadata).ToCamelCase() && (o.Path.Segments[1] == nameof(ResourceMetadata.Annotations).ToCamelCase()
             || o.Path.Segments[1] == nameof(ResourceMetadata.Labels).ToCamelCase())) || o.Path.Segments.First() == nameof(ISpec.Spec).ToCamelCase()));

        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceReference));
        var updatedResource = jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>()!)!;
        if (originalResource.Metadata.ResourceVersion != resource.ConvertTo<Resource>()!.Metadata.ResourceVersion) throw new HyloException(ProblemDetails.ResourceOptimisticConcurrencyCheckFailed(resourceReference, resource.Metadata.ResourceVersion, originalResource.Metadata.ResourceVersion));

        return await this.WriteResourceAsync(group, version, plural, updatedResource, true, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(subResource)) throw new ArgumentNullException(nameof(subResource));

        var resourceReference = new SubResourceReference(new(group, version, plural), name, subResource, @namespace);
        var originalResource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));
        var updatedResource = patch.ApplyTo(originalResource.ConvertTo<Resource>()!)!;

        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalResource, updatedResource);
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => o.Path.Segments.First() == nameof(IStatus.Status).ToCamelCase()));
        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceReference));

        return await this.WriteResourceAsync(group, version, plural, jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>()!)!, false, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(subResource)) throw new ArgumentNullException(nameof(subResource));

        var resourceReference = new SubResourceReference(new(group, version, plural), name, subResource, @namespace);
        var originalResource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));
        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalResource, resource);
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => o.Path.Segments.First() == nameof(IStatus.Status).ToCamelCase()));

        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified((ResourceReference)originalResource!));
        var updatedResource = jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>()!)!;
        if (originalResource.Metadata.ResourceVersion != resource.ConvertTo<Resource>()!.Metadata.ResourceVersion) throw new HyloException(ProblemDetails.ResourceOptimisticConcurrencyCheckFailed(resourceReference, resource.Metadata.ResourceVersion, originalResource.Metadata.ResourceVersion));

        return await this.WriteResourceAsync(group, version, plural, updatedResource, false, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false); ;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> DeleteResourceAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var resource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));
        var key = this.BuildResourceKey(group, version, plural, name, @namespace);
        var namespaceIndexKey = this.BuildResourceByNamespaceIndexKey(@namespace);
        var namespaceIndexEntryKey = name;

        var transactions = new List<Transaction>();

        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            transactions.Add(new()
            {
                OnCommit = () => this.Database.HashDeleteAsync(namespaceIndexKey, namespaceIndexEntryKey)
            });
        }

        transactions.Add(new()
        {
            OnCommit = () => this.Database.KeyDeleteAsync(key)
        });

        var json = Serializer.Json.Serialize(new ResourceWatchEvent(ResourceWatchEventType.Deleted, resource.ConvertTo<Resource>()!));
        transactions.Add(new()
        {
            OnCommit = () => this.Database.PublishAsync(WatchEventChannel, json)
        });

        await transactions.ToAsyncEnumerable().ForEachAwaitAsync(t => t.CommitAsync(), cancellationToken).ConfigureAwait(false);

        return resource;
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="version">The version of the API the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <param name="name">The name of the resource to build the key for</param>
    /// <param name="namespace">The namespace the resource to build the key for belongs to</param>
    /// <returns>The specified resource's key</returns>
    protected virtual string BuildResourceKey(string group, string version, string plural, string name, string? @namespace)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(@namespace)) return $"{group}/{version}/{plural}/{{cluster}}/{name}";
        else return $"{group}/{version}/{plural}/namespaced/{{{@namespace}}}/{name}";
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters for the hashset used to map resources by definition
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <returns>A new resource key</returns>
    protected virtual string BuildResourceByDefinitionIndexKey(string group, string plural)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return $"{plural}.{group}";
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters for the hashset entry used to map resources by definition
    /// </summary>
    /// <param name="name">The name of the resource to build the key for</param>
    /// <param name="namespace">The namespace the resource to build the key for belongs to</param>
    /// <returns>A new resource key</returns>
    protected virtual string BuildResourceByDefinitionIndexEntryKey(string name, string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return string.IsNullOrWhiteSpace(@namespace) ? $"{ClusterResourcePrefix}.{name}" : $"{@namespace}.{name}";
    }

    /// <summary>
    /// Builds a new namespaced resource index key
    /// </summary>
    /// <param name="namespace">The namespace to build a new namespaced resource index key for</param>
    /// <returns>A new namespaced resource index key</returns>
    protected virtual string BuildResourceByNamespaceIndexKey(string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(@namespace)) @namespace = Namespace.DefaultNamespaceName;
        return @namespace;
    }

    /// <summary>
    /// Writes the specified <see cref="Resource"/> to Redis
    /// </summary>
    /// <param name="group">The group the resource to write belongs to</param>
    /// <param name="version">The version of the definition of the resource to write belongs to</param>
    /// <param name="plural">The plural name of the definition of the resource to write belongs to</param>
    /// <param name="resource">The <see cref="Resource"/> to write</param>
    /// <param name="specHasChanged">A boolean indicating whether or not the spec has been updated</param>
    /// <param name="operationType">The type of the write operation to perform</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The persisted <see cref="Resource"/></returns>
    protected virtual async Task<Resource> WriteResourceAsync(string group, string version, string plural, Resource resource, bool specHasChanged, ResourceWatchEventType operationType, CancellationToken cancellationToken = default)
    {
        var key = this.BuildResourceKey(group, version, plural, resource.GetName(), resource.GetNamespace());
        var definitionIndexKey = this.BuildResourceByDefinitionIndexKey(group, plural);
        var definitionIndexEntryKey = this.BuildResourceByDefinitionIndexEntryKey(resource.GetName(), resource.GetNamespace());
        var namespaceIndexKey = this.BuildResourceByNamespaceIndexKey(resource.GetNamespace());
        var namespaceIndexEntryKey = resource.GetName();
        var transactions = new List<Transaction>();

        if (!await this.Database.HashExistsAsync(definitionIndexKey, definitionIndexEntryKey).ConfigureAwait(false))
        {
            transactions.Add(new()
            {
                OnCommit = () => this.Database.HashSetAsync(definitionIndexKey, definitionIndexEntryKey, key, When.NotExists),
                OnRollback = () => this.Database.HashDeleteAsync(definitionIndexKey, definitionIndexEntryKey)
            });
        }

        if (resource.IsNamespaced())
        {
            transactions.Add(new()
            {
                OnCommit = () => this.Database.HashSetAsync(namespaceIndexKey, namespaceIndexEntryKey, key),
                OnRollback = () => this.Database.HashDeleteAsync(namespaceIndexKey, namespaceIndexEntryKey)
            });
        }

        var resourceNode = Serializer.Json.SerializeToNode<object>(resource)!.AsObject();
        resourceNode.Remove(nameof(IMetadata.Metadata).ToCamelCase());
        var json = Serializer.Json.Serialize(resourceNode);
        if (specHasChanged) resource.Metadata.Generation++;
        resource.Metadata.ResourceVersion = string.Format("{0:X}", json.GetHashCode());
        transactions.Add(new()
        {
            OnCommit = () => this.Database.StringSetAsync(key, Serializer.Json.Serialize<object>(resource)),
            OnRollback = () => this.Database.KeyDeleteAsync(key)
        });
        transactions.Add(new()
        {
            OnCommit = () => this.Database.PublishAsync(WatchEventChannel, Serializer.Json.Serialize(new ResourceWatchEvent(operationType, resource)))
        });
        var processed = new List<Transaction>(transactions.Count);
        for (int i = 0; i < transactions.Count; i++)
        {
            var transaction = transactions[i];
            try
            {
                await transaction.CommitAsync();
                processed.Add(transaction);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                processed.Reverse();
                processed.ForEach(async t => await t.RollbackAsync());
                throw new Exception("An error occured while committing pending transactions. Changes have been rolled back.", ex);
            }
        }
        return resource;
    }

    /// <summary>
    /// Reads a <see cref="Resource"/> from Redis
    /// </summary>
    /// <param name="key">The Redis key of the resource to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified <see cref="Resource"/></returns>
    protected virtual async Task<Resource?> ReadResourceAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        var json = (string?)await this.Database.StringGetAsync(key).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json)) return null;
        return Serializer.Json.Deserialize<Resource>(json);
    }

    /// <summary>
    /// Handles a watch event published on Redis
    /// </summary>
    /// <param name="channel">The channel the watch event has been published to</param>
    /// <param name="json">The <see cref="RedisValue"/> that wraps the JSON of the published watch event</param>
    protected virtual void OnWatchEvent(RedisChannel channel, RedisValue json)
    {
        if (channel != WatchEventChannel) return;
        var e = Serializer.Json.Deserialize<ResourceWatchEvent>(json.ToString())!;
        this.ResourceWatchEvents.OnNext(e);
    }

    /// <summary>
    /// Disposes of the <see cref="RedisDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="RedisDatabase"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        this.ResourceWatchEvents.Dispose();
        await this.Subscriber.UnsubscribeAsync(WatchEventChannel, this.OnWatchEvent).ConfigureAwait(false);
        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="RedisDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="RedisDatabase"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource?.Dispose();
                this.ResourceWatchEvents.Dispose();
                this.Subscriber.Unsubscribe(WatchEventChannel, this.OnWatchEvent);
            }
            this._disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Represents a <see cref="RedisDatabase"/> transaction
    /// </summary>
    protected class Transaction
    {

        /// <summary>
        /// Gets/sets the <see cref="Func{TResult}"/> to execute when committing the <see cref="Transaction"/>
        /// </summary>
        public Func<Task>? OnCommit { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Func{TResult}"/> to execute when rolling back the <see cref="Transaction"/>
        /// </summary>
        public Func<Task>? OnRollback { get; set; }

        /// <summary>
        /// Commits the <see cref="Transaction"/>
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public Task CommitAsync() => this.OnCommit == null ? Task.CompletedTask : OnCommit();

        /// <summary>
        /// Rolls back the <see cref="Transaction"/>
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public Task RollbackAsync() => this.OnRollback == null ? Task.CompletedTask : OnRollback();

    }

}