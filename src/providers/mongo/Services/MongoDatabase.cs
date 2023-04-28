using Hylo.Infrastructure.Services;
using Hylo.Resources;
using Hylo.Resources.Definitions;
using Json.Patch;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace Hylo.Providers.Mongo;

/// <summary>
/// Represents a <see href="https://www.mongodb.com/">MongoDB</see> implementation of the <see cref="IDatabase"/> interface
/// </summary>
public class MongoDatabase
    : IDatabase
{

    /// <summary>
    /// Gets the name of the <see cref="MongoDatabase"/>'s connection string
    /// </summary>
    public const string ConnectionStringName = "mongo";
    const string DatabasePrefix = "hylo-";

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="MongoDatabase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="mongo">The service used to interact with MongoDB</param>
    public MongoDatabase(ILoggerFactory loggerFactory, IMongoClient mongo)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Mongo = mongo;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to interact with MongoDB
    /// </summary>
    protected IMongoClient Mongo { get; }

    /// <summary>
    /// Gets the <see cref="MongoDatabase"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <inheritdoc/>
    public virtual async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var initialized = false;
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
    public virtual Task<IResource> CreateResourceAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return this.WriteResourceAsync(resource.ConvertTo<Resource>()!, group, version, plural, true, ResourceWatchEventType.Created, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<IResource?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return this.ReadResourceAsync(group, version, plural, name, @namespace, cancellationToken);
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
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            var namespaces = (await (await this.Mongo.ListDatabaseNamesAsync(cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false))
                .Where(db => db.StartsWith(DatabasePrefix))
                .Select(db => db[DatabasePrefix.Length..])
                .ToList();
            foreach (var ns in namespaces)
            {
                await foreach (var resource in this.GetResourcesAsync(group, version, plural, ns, labelSelectors, cancellationToken).ConfigureAwait(false))
                {
                    yield return resource;
                }
            }
        }
        var collection = this.GetMongoCollection(group, version, plural, @namespace);
        if (labelSelectors == null || !labelSelectors.Any())
        {
            foreach (var resource in collection.AsQueryable().Select(ReadResourceFromBsonDocument<Resource>))
            {
                yield return resource;
            }
        }
        else
        {
            var cursor = await collection.FindAsync(labelSelectors.ToMongoQuery(), cancellationToken: cancellationToken).ConfigureAwait(false);
            foreach (var resource in (await cursor.ToListAsync(cancellationToken).ConfigureAwait(false)).Select(ReadResourceFromBsonDocument<Resource>))
            {
                yield return resource;
            }
        }

    }

    /// <inheritdoc/>
    public virtual async Task<IResourceWatch> WatchResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            var namespaces = (await (await this.Mongo.ListDatabaseNamesAsync(cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false))
                .Where(db => db.StartsWith(DatabasePrefix))
                .Select(db => db[DatabasePrefix.Length..])
                .ToList();
            var tasks = new List<Task<IResourceWatch>>(namespaces.Count);
            foreach (var ns in namespaces)
            {
                tasks.Add(this.WatchResourcesAsync(group, version, plural, ns, labelSelectors, cancellationToken));
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return new MongoCompositeResourceWatch(tasks.Select(t => (MongoResourceWatch)t.Result));
        }
        var collection = this.GetMongoCollection(group, version, plural, @namespace);
        var changeStreamOptions = new ChangeStreamOptions() { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
        var cursor = string.IsNullOrWhiteSpace(@namespace) ?
            await collection.Database.WatchAsync(changeStreamOptions, cancellationToken).ConfigureAwait(false)
            : await collection.WatchAsync(changeStreamOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        return new MongoResourceWatch(cursor, this.CancellationTokenSource);
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
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => o.Path.Segments.First() == nameof(ISpec.Spec).ToCamelCase()));
        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceReference));

        return await this.WriteResourceAsync(jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>()!)!, group, version, plural, true, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false);
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
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => o.Path.Segments.First() == nameof(ISpec.Spec).ToCamelCase()));
        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceReference));

        var updatedResource = jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>()!)!;
        if (originalResource.Metadata.ResourceVersion != resource.ConvertTo<Resource>()!.Metadata.ResourceVersion) throw new Exception("Conflict"); //todo: urgent: replace with proper exception

        return await this.WriteResourceAsync(updatedResource, group, version, plural, true, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var originalResource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));
        var updatedResource = patch.ApplyTo(originalResource.ConvertTo<Resource>()!)!;

        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalResource, updatedResource);
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => o.Path.Segments.First() == subResource));
        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceReference));

        return await this.WriteResourceAsync(jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>()!)!, group, version, plural, false, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var originalResource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));

        var jsonPatch = JsonPatchHelper.CreateJsonPatchFromDiff(originalResource, resource);
        jsonPatch = new JsonPatch(jsonPatch.Operations.Where(o => o.Path.Segments.First() == subResource));
        if (!jsonPatch.Operations.Any()) throw new HyloException(ProblemDetails.ResourceNotModified(resourceReference));

        var updatedResource = jsonPatch.ApplyTo(originalResource.ConvertTo<Resource>())!;
        if (originalResource.Metadata.ResourceVersion != resource.ConvertTo<Resource>()!.Metadata.ResourceVersion) throw new Exception("Conflict"); //todo: urgent: replace with proper exception

        return await this.WriteResourceAsync(updatedResource, group, version, plural, false, ResourceWatchEventType.Updated, cancellationToken).ConfigureAwait(false); ;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> DeleteResourceAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);
        var resource = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new HyloException(ProblemDetails.ResourceNotFound(resourceReference));

        var collection = this.GetMongoCollection(group, version, plural, @namespace);
        await collection.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", name), Builders<BsonDocument>.Update.Set("_deleted", true), cancellationToken: cancellationToken).ConfigureAwait(false); //this trick is used to get the deleted resource in the watch events produced by Mongo
        await Task.Delay(5, cancellationToken);
        await collection.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", name), cancellationToken: cancellationToken).ConfigureAwait(false);

        return resource;
    }

    /// <summary>
    /// Writes the specified <see cref="Resource"/> to Mongo
    /// </summary>
    /// <param name="resource">The <see cref="Resource"/> to write</param>
    /// <param name="group">The group the resource to write belongs to</param>
    /// <param name="version">The version of the definition of the resource to write belongs to</param>
    /// <param name="plural">The plural name of the definition of the resource to write belongs to</param>
    /// <param name="specHasChanged">A boolean indicating whether or not the spec has been updated</param>
    /// <param name="operationType">The type of the write operation to perform</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The persisted <see cref="Resource"/></returns>
    protected virtual async Task<IResource> WriteResourceAsync(Resource resource, string group, string version, string plural, bool specHasChanged, ResourceWatchEventType operationType, CancellationToken cancellationToken = default)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        var resourceNode = Serializer.Json.SerializeToNode<object>(resource)!.AsObject();
        resourceNode.Remove(nameof(IMetadata.Metadata).ToCamelCase());
        var json = Serializer.Json.Serialize(resourceNode);
        if (specHasChanged) resource.Metadata.Generation++;
        resource.Metadata.ResourceVersion = string.Format("{0:X}", json.GetHashCode());

        var collection = this.GetMongoCollection(group, version, plural, resource.GetNamespace());
        json = Serializer.Json.Serialize<object>(resource);
        var document = BsonDocument.Parse(json);
        document["_id"] = resource.GetName();

        switch (operationType)
        {
            case ResourceWatchEventType.Created:
                var insertOptions = new InsertOneOptions();
                await collection.InsertOneAsync(document, insertOptions, cancellationToken).ConfigureAwait(false);
                break;
            case ResourceWatchEventType.Updated:
                var replaceOptions = new ReplaceOptions();
                await collection.ReplaceOneAsync(Builders<BsonDocument>.Filter.Eq("_id", resource.GetName()), document, replaceOptions, cancellationToken).ConfigureAwait(false);
                break;
        }
        return resource;
    }

    /// <summary>
    /// Reads the specified <see cref="IResource"/> from Mongo
    /// </summary>
    /// <param name="group">The group the <see cref="IResource"/> to read belongs to</param>
    /// <param name="version">The version of the api group the <see cref="IResource"/> to read belongs to</param>
    /// <param name="plural">The plural name of the type of the <see cref="IResource"/> to read</param>
    /// <param name="name">The name of the resource to read</param>
    /// <param name="namespace">The namespace the resource to read belongs to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IResource"/> read from Mongo, if any</returns>
    protected virtual async Task<IResource?> ReadResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var collection = this.GetMongoCollection(group, version, plural, @namespace);
        var document = (await collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", name), cancellationToken: cancellationToken).ConfigureAwait(false)).FirstOrDefault(cancellationToken: cancellationToken);
        if (document == null) return null;

        return ReadResourceFromBsonDocument<Resource>(document);
    }

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="IResource"/>s of the the specified type
    /// </summary>
    /// <param name="group">The group of <see cref="IResource"/>s to get the <see cref="IMongoCollection{TDocument}"/> for</param>
    /// <param name="version">The version of <see cref="IResource"/>s to get the <see cref="IMongoCollection{TDocument}"/> for</param>
    /// <param name="plural">The plural name of <see cref="IResource"/>s to get the <see cref="IMongoCollection{TDocument}"/> for</param>
    /// <param name="namespace">The namespace <see cref="IResource"/>s to get the <see cref="IMongoCollection{TDocument}"/> for belong to</param>
    /// <returns>The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="IResource"/>s of the the specified type</returns>
    protected virtual IMongoCollection<BsonDocument> GetMongoCollection(string group, string version, string plural, string? @namespace)
    {
        var database = this.GetMongoDatabase(@namespace);
        var collectionName = $"{group}.{version}.{plural}";
        return database.GetCollection<BsonDocument>(collectionName);
    }

    /// <summary>
    /// Gets the <see cref="IMongoDatabase"/> for the specified namespace
    /// </summary>
    /// <param name="namespace">The namespace to get the <see cref="IMongoDatabase"/> for</param>
    /// <returns>The <see cref="IMongoDatabase"/> for the specified namespace</returns>
    protected virtual IMongoDatabase GetMongoDatabase(string? @namespace = null)
    {
        var databaseName = $"{DatabasePrefix}{(string.IsNullOrWhiteSpace(@namespace) ? "cluster" : @namespace)}";
        return this.Mongo.GetDatabase(databaseName);
    }

    /// <summary>
    /// Reads a <see cref="Resource"/> from the specified <see cref="BsonDocument"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to read</typeparam>
    /// <param name="bsonDocument"></param>
    /// <returns>The deserialized <see cref="Resource"/></returns>
    public static TResource ReadResourceFromBsonDocument<TResource>(BsonDocument bsonDocument)
        where TResource : class, IResource, new()
    {
        bsonDocument.Remove("_id");
        var writerSettings = new JsonWriterSettings() { OutputMode = JsonOutputMode.RelaxedExtendedJson };
        var json = bsonDocument.ToJson(writerSettings);
        return Serializer.Json.Deserialize<TResource>(json)!;
    }

    /// <summary>
    /// Disposes of the <see cref="MongoDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoDatabase"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return ValueTask.CompletedTask;
        this.CancellationTokenSource.Dispose();
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
    /// Disposes of the <see cref="MongoDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoDatabase"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}