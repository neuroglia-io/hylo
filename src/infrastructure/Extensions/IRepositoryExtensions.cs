﻿namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="IRepository"/> implementations
/// </summary>
public static class IRepositoryExtensions
{

    /// <summary>
    /// Gets the resource definition with the specified name
    /// </summary>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="group">The API group the resource definition to get belongs to</param>
    /// <param name="plural">The plural name of the resource definition to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static async Task<IResourceDefinition?> GetDefinitionAsync(this IRepository repository, string group, string plural, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        var resource = await repository.GetAsync<ResourceDefinition>(string.IsNullOrWhiteSpace(group) ? plural : $"{plural}.{group}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if(resource == null) return null;
        return resource.ConvertTo<ResourceDefinition>();
    }

    /// <summary>
    /// Gets the definition of the specified resource type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get the definition of</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static Task<IResourceDefinition?> GetDefinitionAsync<TResource>(this IRepository repository, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        return repository.GetDefinitionAsync(resource.GetGroup(), resource.Definition.Plural, cancellationToken);
    }

    /// <summary>
    /// Lists <see cref="IResourceDefinition"/>s
    /// </summary>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResourceDefinition"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resource definitions, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResourceDefinition"/>s</returns>
    public static async Task<ICollection<ResourceDefinition>> ListDefinitionsAsync(this IRepository repository, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        return await repository.ListAsync<ResourceDefinition>(null, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to add</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="resource">The <see cref="IResource"/> to add</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The added <see cref="IResource"/></returns>
    public static async Task<TResource> AddAsync<TResource>(this IRepository repository, TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if(resource == null) throw new ArgumentNullException(nameof(resource));
        var result = await repository.AddAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.GetNamespace(), dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Adds a new <see cref="Namespace"/>
    /// </summary>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="name">The name of the <see cref="Namespace"/> to add</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The added <see cref="Namespace"/></returns>
    public static async Task<Namespace> AddNamespaceAsync(this IRepository repository, string name, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return await repository.AddAsync<Namespace>(new(name), dryRun, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the <see cref="IResource"/> with the specified name, if any
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to get belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IResource"/> with the specified name, if any</returns>
    public static async Task<TResource?> GetAsync<TResource>(this IRepository repository, string name, string? @namespace = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resource = new TResource();
        var result = await repository.GetAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Lists <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to list</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to list belongs to, if any. If not set, lists resources accross all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resources, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResource"/>s of type specified type</returns>
    public static async Task<ICollection<TResource>> ListAsync<TResource>(this IRepository repository, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var collection = await repository.ListAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
        return collection.OfType<TResource>();
    }

    /// <summary>
    /// Streams <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to stream</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any. If not set, streams resources accross all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to stream by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to stream all matching <see cref="IResource"/>s of type specified type</returns>
    public static IAsyncEnumerable<TResource> GetAllAsync<TResource>(this IRepository repository, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var stream = repository.GetAllAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, cancellationToken);
        return stream.Select(r => r.ConvertTo<TResource>()!);
    }

    /// <summary>
    /// Observes events produced by <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to observe</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any. If not set, observes resources accross all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to observe by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A <see cref="IResourceWatch{TResource}"/> used to observe events produced by <see cref="IResource"/>s of the specified type</returns>
    public static async Task<IResourceWatch<TResource>> WatchAsync<TResource>(this IRepository repository, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var watch = await repository.WatchAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, cancellationToken).ConfigureAwait(false);
        return watch.OfType<TResource>();
    }

    /// <summary>
    /// Patches the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to patch</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="name">The name of the <see cref="IResource"/> to patch</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> PatchAsync<TResource>(this IRepository repository, Patch patch, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        var resource = new TResource();
        var result = await repository.PatchAsync(patch, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Replaces the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="resource">The state to replace the <see cref="IResource"/> with</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> ReplaceAsync<TResource>(this IRepository repository, TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        var result = await repository.ReplaceAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.GetName(), resource.GetNamespace(), dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Patches the specified <see cref="IResource"/>'s status
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to patch the status of</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="name">The name of the <see cref="IResource"/> to patch the status of</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch the status of belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> PatchStatusAsync<TResource>(this IRepository repository, Patch patch, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (patch == null) throw new ArgumentNullException(nameof(patch));
        var resource = new TResource();
        var result = await repository.PatchSubResourceAsync(patch, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, "status", @namespace, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Replaces the specified <see cref="IResource"/>'s status
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace the status with</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="resource">The state to replace the <see cref="IResource"/> the status with</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> ReplaceStatusAsync<TResource>(this IRepository repository, TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        var result = await repository.ReplaceSubResourceAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.GetName(), "status", resource.GetNamespace(), dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Removes the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to remove</typeparam>
    /// <param name="repository">The extended <see cref="IRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to remove</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to remove belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The removed <see cref="IResource"/></returns>
    public static async Task<TResource> RemoveAsync<TResource>(this IRepository repository, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resource = new TResource();
        var result = await repository.RemoveAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Gets all <see cref="MutatingWebhook"/>s that apply to the specified resource and operation
    /// </summary>
    /// <param name="resources">The <see cref="IRepository"/> to query</param>
    /// <param name="operation">The operation for which to retrieve matching <see cref="MutatingWebhook"/>s</param>
    /// <param name="resource">An object used to reference the resource to get <see cref="MutatingWebhook"/>s for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing all matching <see cref="MutatingWebhook"/>s</returns>
    public static IAsyncEnumerable<MutatingWebhook> GetMutatingWebhooksFor(this IRepository resources, Operation operation, IResourceReference resource, CancellationToken cancellationToken = default)
    {
        return resources
            .GetAllAsync<MutatingWebhook>(cancellationToken: cancellationToken)
            .Where(wh => wh.Spec.Resources == null || wh.Spec.Resources.Any(r => r.Matches(operation, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.Namespace)));
    }

    /// <summary>
    /// Gets all <see cref="ValidatingWebhook"/>s that apply to the specified resource and operation
    /// </summary>
    /// <param name="resources">The <see cref="IRepository"/> to query</param>
    /// <param name="operation">The operation for which to retrieve matching <see cref="ValidatingWebhook"/>s</param>
    /// <param name="resource">An object used to reference the resource to get <see cref="ValidatingWebhook"/>s for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing all matching <see cref="ValidatingWebhook"/>s</returns>
    public static IAsyncEnumerable<ValidatingWebhook> GetValidatingWebhooksFor(this IRepository resources, Operation operation, IResourceReference resource, CancellationToken cancellationToken = default)
    {
        return resources
            .GetAllAsync<ValidatingWebhook>(cancellationToken: cancellationToken)
            .Where(wh => wh.Spec.Resources == null || wh.Spec.Resources.Any(r => r.Matches(operation, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.Namespace)));
    }

}
