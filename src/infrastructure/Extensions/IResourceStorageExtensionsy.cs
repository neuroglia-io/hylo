namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="IResourceStorage"/> implementations
/// </summary>
public static class IResourceStorageExtensions
{

    /// <summary>
    /// Gets the resource definition with the specified name
    /// </summary>
    /// <param name="storage">The extended <see cref="IResourceStorage"/></param>
    /// <param name="group">The API group the resource definition to get belongs to</param>
    /// <param name="plural">The plural name of the resource definition to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static async Task<IResourceDefinition?> ReadOneDefinitionAsync(this IResourceStorage storage, string group, string plural, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        var resource = await storage.ReadOneAsync<ResourceDefinition>(string.IsNullOrWhiteSpace(group) ? plural :$"{plural}.{group}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if(resource == null) return null;
        return resource.ConvertTo<ResourceDefinition>();
    }

    /// <summary>
    /// Gets the definition of the specified resource type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get the definition of</typeparam>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static Task<IResourceDefinition?> ReadOneDefinitionAsync<TResource>(this IResourceStorage storage, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        return storage.ReadOneDefinitionAsync(resource.GetGroup(), resource.Definition.Plural, cancellationToken);
    }

    /// <summary>
    /// Lists <see cref="IResourceDefinition"/>s
    /// </summary>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResourceDefinition"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resource definitions, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResourceDefinition"/>s</returns>
    public static async Task<ICollection<ResourceDefinition>> ReadDefinitionsAsync(this IResourceStorage storage, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        return await storage.ReadAsync<ResourceDefinition>(null, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to add</typeparam>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="resource">The <see cref="IResource"/> to add</param>
    /// <param name="subResource">A reference to the sub resource to write, if any</param>
    /// <param name="ifNotExists">A boolean indicating whether or not to abstrain from writing when the resource already exists</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The added <see cref="IResource"/></returns>
    public static async Task<TResource> WriteAsync<TResource>(this IResourceStorage storage, TResource resource, string? subResource = null, bool ifNotExists = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if(resource == null) throw new ArgumentNullException(nameof(resource));
        var result = await storage.WriteAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.GetNamespace(), subResource, ifNotExists, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Adds a new <see cref="Namespace"/>
    /// </summary>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="name">The name of the <see cref="Namespace"/> to add</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The added <see cref="Namespace"/></returns>
    public static async Task<Namespace> WriteNamespaceAsync(this IResourceStorage storage, string name, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return await storage.WriteAsync<Namespace>(new(name), null, true, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the <see cref="IResource"/> with the specified name, if any
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get</typeparam>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to get belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IResource"/> with the specified name, if any</returns>
    public static async Task<TResource?> ReadOneAsync<TResource>(this IResourceStorage storage, string name, string? @namespace = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resource = new TResource();
        var result = await storage.ReadOneAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Lists <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to list</typeparam>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to list belongs to, if any. If not set, lists resources accross all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resources, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResource"/>s of type specified type</returns>
    public static async Task<ICollection<TResource>> ReadAsync<TResource>(this IResourceStorage storage, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var collection = await storage.ReadAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
        return collection.OfType<TResource>();
    }

    /// <summary>
    /// Streams <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to stream</typeparam>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any. If not set, streams resources accross all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to stream by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to stream all matching <see cref="IResource"/>s of type specified type</returns>
    public static IAsyncEnumerable<TResource> ReadAllAsync<TResource>(this IResourceStorage storage, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var stream = storage.ReadAllAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, cancellationToken);
        return stream.Select(r => r.ConvertTo<TResource>()!);
    }

    /// <summary>
    /// Observes events produced by <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to observe</typeparam>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any. If not set, observes resources accross all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to observe by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A <see cref="IResourceWatch{TResource}"/> used to observe events produced by <see cref="IResource"/>s of the specified type</returns>
    public static async Task<IResourceWatch<TResource>> WatchAsync<TResource>(this IResourceStorage storage, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var watch = await storage.WatchAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, cancellationToken).ConfigureAwait(false);
        return watch.OfType<TResource>();
    }

    /// <summary>
    /// Deletes the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to delete</typeparam>
    /// <param name="storage">The extended <see cref="IResourceRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to delete</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to delete belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    public static async Task<TResource> DeleteAsync<TResource>(this IResourceStorage storage, string name, string? @namespace = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resource = new TResource();
        var result = await storage.DeleteAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

}
