using Hylo.Api.Core.Data;
using Hylo.Api.Core.Data.Models;
using Hylo.Serialization.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the REDIS implementation of the <see cref="IResourceRepository"/> interface
/// </summary>
public class RedisResourceRepository
    : IResourceRepository
{

    /// <summary>
    /// Initializes a new <see cref="RedisResourceRepository"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="redis">The current <see cref="IConnectionMultiplexer"/></param>
    public RedisResourceRepository(ILoggerFactory loggerFactory, IConnectionMultiplexer redis)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Redis = redis;
        this.Database = redis.GetDatabase();
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
    /// Gets the current <see cref="IDatabase"/>
    /// </summary>
    protected IDatabase Database { get; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing all pending <see cref="Transaction"/>s
    /// </summary>
    protected List<Transaction> PendingTransactions { get; } = new();

    /// <inheritdoc/>
    public virtual async Task<object> AddResourceAsync(string group, string version, string plural, object resource, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        var resourceObject = Serializer.Json.SerializeToNode(resource)!.AsObject()!;

        var resourceName = resourceObject.GetResourceName()!;
        var resourceNamespace = resourceObject.GetResourceNamespace();
        var resourceKey = this.BuildResourceKey(group, version, plural, resourceName, resourceNamespace);
        var resourceByApiVersionIndexKey = this.BuildResourceByApiVersionIndexKey(group, version, plural);
        var resourceByApiVersionIndexEntryKey = this.BuildResourceByApiVersionIndexEntryKey(resourceName, resourceNamespace);
        this.PendingTransactions.Add(new()
        {
            OnCommit = async () =>
            {
                return await this.Database.HashSetAsync(resourceByApiVersionIndexKey, resourceByApiVersionIndexEntryKey, resourceKey, When.NotExists);
            },
            OnRollback = async () =>
            {
                await this.Database.HashDeleteAsync(resourceByApiVersionIndexKey, resourceByApiVersionIndexEntryKey);
            }
        });

        resource = await this.PersistResourceAsync(group, version, plural, resourceObject, When.NotExists, cancellationToken);
        return resource;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ContainsResourceAsync(string group, string version, string plural, string name, string? @namespace, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resourceByApiVersionIndexKey = this.BuildResourceByApiVersionIndexKey(group, version, plural);
        var resourceByApiVersionIndexEntryKey = this.BuildResourceByApiVersionIndexEntryKey(name, @namespace);
        return await this.Database.HashExistsAsync(resourceByApiVersionIndexKey, resourceByApiVersionIndexEntryKey);
    }

    /// <inheritdoc/>
    public virtual async Task<V1ResourceDefinition?> GetResourceDefinitionAsync(string group, string version, string plural, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        var definitionByQualifiedPluralNameIndexKey = this.BuildDefinitionByQualifiedPluralNameIndexKey(group, version, plural);
        var resourceKey = (string?)(await this.Database.StringGetAsync(definitionByQualifiedPluralNameIndexKey));
        if (string.IsNullOrWhiteSpace(resourceKey)) return null;
        return (await this.MaterializeAsync(resourceKey)).Deserialize<V1ResourceDefinition>();
    }

    /// <inheritdoc/>
    public virtual async Task<object?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resourceByApiVersionIndexKey = this.BuildResourceByApiVersionIndexKey(group, version, plural);
        var resourceByApiVersionIndexEntryKey = this.BuildResourceByApiVersionIndexEntryKey(name, @namespace);
        var resourceKey = (string)(await this.Database.HashGetAsync(resourceByApiVersionIndexKey, resourceByApiVersionIndexEntryKey))!;
        return await this.MaterializeAsync(resourceKey);
    }

    /// <inheritdoc/>
    public virtual async Task<TResource?> GetResourceAsync<TResource>(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default) 
        where TResource : V1Resource
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var jsonObject = (JsonObject?)await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken);
        if (jsonObject == null) return null;
        return Serializer.Json.Deserialize<TResource>(jsonObject);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<object> ListResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<string>? labelSelectors = null,
        int resultsPerPage = V1CoreApiDefaults.Paging.MaxResultsPerPage, string? orderBy = null, bool orderByDescending = false, int? pageIndex = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            string pattern;
            if (string.IsNullOrWhiteSpace(@namespace)) pattern = "cluster.*";
            else pattern = $"{@namespace}.*";
            var resourceByApiVersionIndexKey = this.BuildResourceByApiVersionIndexKey(group, version, plural);
            var keyEntryScan = this.Database.HashScanAsync(resourceByApiVersionIndexKey, pattern, resultsPerPage, pageOffset: pageIndex ?? 0);
            await foreach (var resourceKeyEntry in keyEntryScan.Take(resultsPerPage))
            {
                var resourceKey = ((string)resourceKeyEntry.Value)!;
                if (labelSelectors != null)
                {
                    var matches = true;
                    foreach (var labelSelector in labelSelectors)
                    {
                        if (!await this.Database.HashScanAsync(resourceKey, BuildLabelFieldKey(labelSelector)).AnyAsync(cancellationToken))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (!matches) continue;
                }
                yield return await this.MaterializeAsync(resourceKey);
            }
        }
        else
        {
            var resourceOrderedByIndexKey = this.BuildOrderByIndexKey(group, version, plural, @namespace, orderBy);
            var skip = (pageIndex.HasValue ? pageIndex.Value : 0) * resultsPerPage;
            var take = resultsPerPage;
            var keys = await this.Database.SortedSetRangeByScoreAsync(resourceOrderedByIndexKey, order: orderByDescending ? Order.Descending : Order.Ascending, skip: skip, take: take);
            foreach (string? resourceKey in keys)
            {
                if (string.IsNullOrWhiteSpace(resourceKey)) continue;
                if (labelSelectors != null)
                {
                    var matches = true;
                    foreach (var labelSelector in labelSelectors)
                    {
                        if (!await this.Database.HashScanAsync(resourceKey, BuildLabelFieldKey(labelSelector)).AnyAsync(cancellationToken))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (!matches) continue;
                }
                yield return await this.MaterializeAsync(resourceKey);
            }
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TResource?> ListResourcesAsync<TResource>(string group, string version, string plural, string? @namespace = null, IEnumerable<string>? labelSelectors = null, 
        int resultsPerPage = V1CoreApiDefaults.Paging.MaxResultsPerPage, string? orderBy = null, bool orderByDescending = false, int? pageIndex = null, [EnumeratorCancellation] CancellationToken cancellationToken = default) 
        where TResource : V1Resource
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        await foreach(var resource in this.ListResourcesAsync(group, version, plural, @namespace, labelSelectors, resultsPerPage, orderBy, orderByDescending, pageIndex, cancellationToken))
        {
            var jsonObject = (JsonObject)resource;
            if (jsonObject == null) continue;
            yield return Serializer.Json.Deserialize<TResource>(jsonObject);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<object> UpdateResourceAsync(string group, string version, string plural, string name, string? @namespace, object resource, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        var resourceObject = Serializer.Json.SerializeToNode(resource)!.AsObject()!;

        resource = await this.PersistResourceAsync(group, version, plural, resourceObject, When.Exists, cancellationToken);
        return resource;
    }

    /// <inheritdoc/>
    public virtual async Task<object> RemoveResourceAsync(string group, string version, string plural, string name, string? @namespace, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceByApiVersionIndexKey = this.BuildResourceByApiVersionIndexKey(group, version, plural);
        var resourceByApiVersionIndexEntryKey = this.BuildResourceByApiVersionIndexEntryKey(name, @namespace);
        var resourceKey = this.BuildResourceKey(group, version, plural, name, @namespace);

        var resourceObject = await this.MaterializeAsync(resourceKey);

        await this.Database.HashDeleteAsync(resourceByApiVersionIndexKey, resourceByApiVersionIndexEntryKey);
        await this.Database.KeyDeleteAsync(resourceKey);

        return resourceObject;
    }

    /// <inheritdoc/>
    public virtual async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!this.PendingTransactions.Any()) return;
        var processed = new List<Transaction>(this.PendingTransactions.Count);
        var transactions = this.PendingTransactions.ToList();
        for (int i = 0; i < transactions.Count; i++)
        {
            var transaction = transactions[i];
            if (!await transaction.CommitAsync())
            {
                await transaction.RollbackAsync();
                processed.Reverse();
                processed.ForEach(async t => await t.RollbackAsync());
                throw new Exception("An error occured while committing pending transactions. Changes have been rolled back.");
            }
            processed.Add(transaction);
            this.PendingTransactions.Remove(transaction);
        }
    }

    /// <summary>
    /// Stores the specified resource
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="version">The version of the API the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <param name="resource">A <see cref="JsonObject"/> that represents the resource to store</param>
    /// <param name="when">Sets the key rule to enforce when storing the resource</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The stored resource</returns>
    protected virtual async Task<JsonObject> PersistResourceAsync(string group, string version, string plural, JsonObject resource, When when, CancellationToken cancellationToken)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (!resource.TryGetResourceMetadata(out var metadata) || metadata == null) throw new Exception();//todo: replace ex
        if (string.IsNullOrWhiteSpace(metadata.Name)) throw new Exception();//todo: replace ex
        var resourceKey = this.BuildResourceKey(group, version, plural, metadata.Name!, metadata.Namespace);

        this.PendingTransactions.Add(this.PersistResourceState(group, version, plural, resourceKey, metadata, resource, when, cancellationToken));

        if (resource.IsResourceDefinition() && when == When.NotExists) this.PendingTransactions.Add(this.PersistResourceDefinitionState(group, version, plural, resourceKey, metadata, resource, when, cancellationToken));

        resource["metadata"] = Serializer.Json.SerializeToNode(metadata);
        return await Task.FromResult(resource);
    }

    /// <summary>
    /// Creates a new <see cref="Transaction"/> used to persist the state of a <see cref="V1Resource"/>
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="version">The version of the API the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <param name="resourceKey">The key of the resource to persist the state of</param>
    /// <param name="metadata">The metadata of the resource to persist the state of</param>
    /// <param name="resource">The resource to persist</param>
    /// <param name="when">Configures the behavior to use when the key already exists</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="Transaction"/></returns>
    protected virtual Transaction PersistResourceState(string group, string version, string plural, string resourceKey, V1ResourceMetadata metadata, JsonObject resource, When when, CancellationToken cancellationToken)
    {

        return new()
        {
            OnCommit = async () =>
            {
                var transaction = this.Database.CreateTransaction();

                if (when == When.Exists)
                {
                    var storedMetadata = Serializer.Json.Deserialize<V1ResourceMetadata>((await this.Database.HashGetAsync(resourceKey, ResourceHashFields.Metadata))!)!;
                    if (metadata.StateVersion.HasValue && metadata.StateVersion.Value != storedMetadata.StateVersion) throw new Exception(); //todo: replace ex
                    metadata.StateVersion = storedMetadata.StateVersion + 1;
                    transaction.AddCondition(Condition.HashEqual(resourceKey, ResourceHashFields.StateVersion, storedMetadata.StateVersion));
                }

                foreach (var orderBy in V1Resource.GetSortableProperties())
                {
                    var resourcesOrderedByIndexKey = this.BuildOrderByIndexKey(group, version, plural, metadata.Namespace, orderBy);
                    var score = this.ComputePropertySortedSetScore(orderBy, resource);
                    _ = transaction.SortedSetAddAsync(resourcesOrderedByIndexKey, resourceKey, score, SortedSetWhen.Always);
                }

                _ = transaction.HashSetAsync(resourceKey, ResourceHashFields.ApiVersion, resource.GetResourceApiVersion());
                _ = transaction.HashSetAsync(resourceKey, ResourceHashFields.Kind, resource.GetResourceKind());
                _ = transaction.HashSetAsync(resourceKey, ResourceHashFields.StateVersion, metadata.StateVersion);
                _ = transaction.HashSetAsync(resourceKey, ResourceHashFields.Metadata, Serializer.Json.Serialize(metadata));
                if (resource.TryGetPropertyValue("spec", out var spec)) _ = transaction.HashSetAsync(resourceKey, ResourceHashFields.Spec, spec?.ToJsonString(Serializer.Json.DefaultOptions));
                if (resource.TryGetPropertyValue("status", out var status)) _ = transaction.HashSetAsync(resourceKey, ResourceHashFields.Status, status?.ToJsonString(Serializer.Json.DefaultOptions));

                if (metadata.Labels != null)
                {
                    foreach (var label in metadata.Labels)
                    {
                        _ = transaction.HashSetAsync(resourceKey, this.BuildLabelFieldKey($"{label.Key}={label.Value}"), label.Value);
                    }
                }

                if (metadata.IsNamespaced())
                {
                    var namespacedResourceIndexKey = this.BuildNamespacedResourceIndexKey(metadata.Namespace!);
                    _ = transaction.HashSetAsync(namespacedResourceIndexKey, metadata.Name, resourceKey);
                }

                return await transaction.ExecuteAsync();
            },
            OnRollback = async () =>
            {
                foreach (var orderBy in V1Resource.GetSortableProperties())
                {
                    var resourcesOrderedByIndexKey = this.BuildOrderByIndexKey(group, version, plural, metadata.Namespace, orderBy);
                    await this.Database.SortedSetRemoveAsync(resourcesOrderedByIndexKey, resourceKey);
                }
                await this.Database.KeyDeleteAsync(resourceKey);
            }
        };
    }

    /// <summary>
    /// Creates a new <see cref="Transaction"/> used to persist the state of a <see cref="V1ResourceDefinition"/>
    /// </summary>
    /// <param name="group">The API group the resource definition to store belongs to</param>
    /// <param name="version">The version of the API the resource definition to store belongs to</param>
    /// <param name="plural">The plural form of the resource definition's type</param>
    /// <param name="resourceKey">The key of the resource definition to persist the state of</param>
    /// <param name="metadata">The metadata of the resource definition to persist the state of</param>
    /// <param name="resource">The resource definition to persist</param>
    /// <param name="when">Configures the behavior to use when the key already exists</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="Transaction"/></returns>
    protected virtual Transaction PersistResourceDefinitionState(string group, string version, string plural, string resourceKey, V1ResourceMetadata metadata, JsonObject resource, When when, CancellationToken cancellationToken)
    {
        if (!resource.TryGetPropertyValue<V1ResourceDefinitionSpec>(nameof(V1Resource<object>.Spec).ToCamelCase(), out var definitionSpec) || definitionSpec == null) throw new Exception(); //todo: replace ex
        var definitionByQualifiedPluralNameIndexKey = this.BuildDefinitionByQualifiedPluralNameIndexKey(definitionSpec.Group, definitionSpec.Version, definitionSpec.Names.Plural);
        return new()
        {
            OnCommit = async () =>
            {
                return await this.Database.StringSetAsync(definitionByQualifiedPluralNameIndexKey, resourceKey, when: when);
            },
            OnRollback = async () =>
            {
                await this.Database.KeyDeleteAsync(definitionByQualifiedPluralNameIndexKey);
            }
        };
    }

    /// <summary>
    /// Materializes the <see cref="V1Resource"/> with the specified key
    /// </summary>
    /// <param name="resourceKey">The key of the <see cref="V1Resource"/> to materialize</param>
    /// <param name="queryOptions">The options used to configure the <see cref="V1Resource"/>'s materialization</param>
    /// <returns>A <see cref="JsonObject"/> that represents the materialized <see cref="V1Resource"/></returns>
    protected virtual async Task<JsonObject> MaterializeAsync(string resourceKey)
    {
        if (string.IsNullOrWhiteSpace(resourceKey)) throw new ArgumentNullException(nameof(resourceKey));
        var resource = new JsonObject();

        var json = (string)(await this.Database.HashGetAsync(resourceKey, ResourceHashFields.Metadata))!;
        resource[nameof(V1Resource.ApiVersion).ToCamelCase()] = (string)(await this.Database.HashGetAsync(resourceKey, ResourceHashFields.ApiVersion))!;
        resource[nameof(V1Resource.Kind).ToCamelCase()] = (string)(await this.Database.HashGetAsync(resourceKey, ResourceHashFields.Kind))!;
        resource[nameof(V1Resource.Metadata).ToCamelCase()] = JsonNode.Parse(json)!.AsObject();

        json = (string)(await this.Database.HashGetAsync(resourceKey, ResourceHashFields.Spec))!;
        if (!string.IsNullOrWhiteSpace(json)) resource[nameof(V1Resource<object>.Spec).ToCamelCase()] = JsonNode.Parse(json)!.AsObject();

        json = (string)(await this.Database.HashGetAsync(resourceKey, ResourceHashFields.Status))!;
        if (!string.IsNullOrWhiteSpace(json)) resource[nameof(V1Resource<object, object>.Status).ToCamelCase()] = JsonNode.Parse(json)!.AsObject();

        return resource;
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters for the hashset used to map resources by API version
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="version">The version of the API the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <returns>A new resource key</returns>
    protected virtual string BuildResourceByApiVersionIndexKey(string group, string version, string plural)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return $"api:{group}/{version}/{plural}";
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters for the hashset entry used to map resources by API version
    /// </summary>
    /// <param name="name">The name of the resource to build the key for</param>
    /// <param name="namespace">The namespace the resource to build the key for belongs to</param>
    /// <returns>A new resource key</returns>
    protected virtual string BuildResourceByApiVersionIndexEntryKey(string name, string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(@namespace)) return $"cluster.{name}";
        else return $"{@namespace}.{name}";
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters for the hashset used to order resources by property
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="version">The version of the API the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <param name="namespace">The namespace the resource to build the key for belongs to</param>
    /// <param name="orderBy">The property to order resources by</param>
    /// <returns>A new resource key</returns>
    protected virtual string BuildOrderByIndexKey(string group, string version, string plural, string? @namespace, string orderBy)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(orderBy)) throw new ArgumentNullException(nameof(orderBy));
        if (string.IsNullOrWhiteSpace(@namespace)) return $"{orderBy}:{group}/{version}/{plural}/{{cluster}}";
        else return $"{orderBy}:{group}/{version}/{plural}/{{{@namespace}}}";
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters for the hashset used to map resource definitions by qualified plural name
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="version">The version of the API the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <returns>A new resource key</returns>
    protected virtual string BuildDefinitionByQualifiedPluralNameIndexKey(string group, string version, string plural)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return $"def:{group}/{version}/{plural}";
    }

    /// <summary>
    /// Builds a new namespaced resource index key
    /// </summary>
    /// <param name="namespace">The namespace to build a new namespaced resource index key for</param>
    /// <returns>A new namespaced resource index key</returns>
    protected virtual string BuildNamespacedResourceIndexKey(string @namespace)
    {
        if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));
        return $"ns:{{{@namespace}}}";
    }

    /// <summary>
    /// Builds a resource key based on the specified parameters
    /// </summary>
    /// <param name="group">The API group the resource to store belongs to</param>
    /// <param name="version">The version of the API the resource to store belongs to</param>
    /// <param name="plural">The plural form of the resource's type</param>
    /// <param name="name">The name of the resource to build the key for</param>
    /// <param name="namespace">The namespace the resource to build the key for belongs to</param>
    /// <returns>A new resource key</returns>
    protected virtual string BuildResourceKey(string group, string version, string plural, string name, string? @namespace)
    {
        if (string.IsNullOrWhiteSpace(@namespace)) return $"{group}/{version}/{plural}/{{cluster}}/{name}";
        else return $"{group}/{version}/{plural}/{{{@namespace}}}/{name}";
    }

    /// <summary>
    /// Builds a new field key for the specified label
    /// </summary>
    /// <param name="label">The label to create a new field key for</param>
    /// <returns>A new field key for the specified label</returns>
    protected virtual string BuildLabelFieldKey(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentNullException(nameof(label));
        return $"{ResourceHashFields.Label}:{label}";
    }

    /// <summary>
    /// Computes the sorted set score of the specified resource property
    /// </summary>
    /// <param name="propertyPath">The path of the property to compute the score of</param>
    /// <param name="resource">The resource to compute</param>
    /// <returns>The sorted set score of the specified resource property</returns>
    protected virtual double ComputePropertySortedSetScore(string propertyPath, JsonObject resource)
    {
        if (string.IsNullOrWhiteSpace(propertyPath)) throw new ArgumentNullException(nameof(propertyPath));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (!resource.TryGetPropertyValue(propertyPath, JsonObjectPropertyReferenceType.Path, out var jsonNode)) return double.MinValue;
        if (jsonNode is not JsonValue jsonValue) throw new ArgumentException("The referenced property must be a value node", nameof(propertyPath));
        var valueKind = jsonNode.AsJsonElement().ValueKind;
        return valueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => double.MinValue,
            JsonValueKind.True => 1,
            JsonValueKind.False => 0,
            JsonValueKind.Number => jsonNode.Deserialize<double>(),
            JsonValueKind.String => this.ComputeStringSortedSetScore(jsonNode.Deserialize<string>()),
            _ => throw new NotSupportedException($"The specified {nameof(JsonValueKind)} '{valueKind}' is not supported in this context")
        };
    }

    /// <summary>
    /// Computes the sorted set score of the specified string
    /// </summary>
    /// <param name="input">The string to check the sorted set score of</param>
    /// <returns>The sorted set score of the specified string</returns>
    protected virtual double ComputeStringSortedSetScore(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return double.MinValue;
        if (DateTimeOffset.TryParse(input, out var dateTime)) return dateTime.Ticks;
        return input.ComputeSortedSetScore();
    }

    /// <summary>
    /// Describes the fields of a <see cref="V1Resource"/>'s REDIS hash
    /// </summary>
    protected static class ResourceHashFields
    {

        /// <summary>
        /// Gets the name of the api version <see cref="HashEntry"/>
        /// </summary>
        public const string ApiVersion = "apiVersion";
        /// <summary>
        /// Gets the name of the kind <see cref="HashEntry"/>
        /// </summary>
        public const string Kind = "kind";
        /// <summary>
        /// Gets the name of the state version <see cref="HashEntry"/>
        /// </summary>
        public const string StateVersion = "stateVersion";
        /// <summary>
        /// Gets the name of the metadata <see cref="HashEntry"/>
        /// </summary>
        public const string Metadata = "metadata";
        /// <summary>
        /// Gets the name of the spec <see cref="HashEntry"/>
        /// </summary>
        public const string Spec = "spec";
        /// <summary>
        /// Gets the name of the status <see cref="HashEntry"/>
        /// </summary>
        public const string Status = "status";
        /// <summary>
        /// Gets the prefix of all label <see cref="HashEntry"/>
        /// </summary>
        public const string Label = "label";

    }

    /// <summary>
    /// Represents a <see cref="RedisResourceRepository"/> transaction
    /// </summary>
    public class Transaction
    {

        /// <summary>
        /// Gets/sets the <see cref="Func{TResult}"/> to execute when committing the <see cref="Transaction"/>
        /// </summary>
        public Func<Task<bool>>? OnCommit { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Func{TResult}"/> to execute when rolling back the <see cref="Transaction"/>
        /// </summary>
        public Func<Task>? OnRollback { get; set; }

        /// <summary>
        /// Commits the <see cref="Transaction"/>
        /// </summary>
        /// <returns>A boolean indicating whether or not the <see cref="Transaction"/> was successfully committed</returns>
        public Task<bool> CommitAsync() => this.OnCommit == null ? Task.FromResult(true) : OnCommit();

        /// <summary>
        /// Rolls back the <see cref="Transaction"/>
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public Task RollbackAsync() => this.OnRollback == null ? Task.CompletedTask : OnRollback();

    }

}
