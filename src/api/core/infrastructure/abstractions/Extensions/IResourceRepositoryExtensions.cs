namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines extensions for <see cref="IResourceRepository"/> instances
/// </summary>
public static class IResourceRepositoryExtensions
{

    /// <summary>
    /// Gets the specified <see cref="V1ResourceDefinition"/>
    /// </summary>
    /// <param name="resources">The <see cref="IResourceRepository"/> to query</param>
    /// <param name="resource">The <see cref="V1Resource"/> to get the definition for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="V1ResourceDefinition"/> with the specified group, version and plural name</returns>
    public static async Task<V1ResourceDefinition?> GetResourceDefinitionAsync(this IResourceRepository resources, V1ResourceReference resource, CancellationToken cancellationToken = default)
    {
        return await resources.GetResourceDefinitionAsync(resource.Group, resource.Version, resource.Plural, cancellationToken);
    }

    /// <summary>
    /// Determines whether or not the specified <see cref="V1Namespace"/> exists
    /// </summary>
    /// <param name="resources">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The namespace to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the specified <see cref="V1Namespace"/> exists</returns>
    public static Task<bool> ContainsNamespaceAsync(this IResourceRepository resources, string @namespace, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));
        return resources.ContainsResourceAsync(V1Namespace.HyloApiVersion, V1Namespace.HyloApiVersion, V1Namespace.HyloPluralName, @namespace, null, cancellationToken);
    }

    /// <summary>
    /// Gets the specified <see cref="V1Namespace"/>
    /// </summary>
    /// <param name="resources">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The name of the <see cref="V1Namespace"/> to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified <see cref="V1Namespace"/>, if any</returns>
    public static async Task<V1Namespace?> GetNamespaceAsync(this IResourceRepository resources, string @namespace, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));
        var resource = await resources.GetResourceAsync(V1Namespace.HyloApiVersion, V1Namespace.HyloApiVersion, V1Namespace.HyloPluralName, @namespace, null, cancellationToken);
        if (resource == null) return null;
        return Serializer.Json.Deserialize<V1Namespace>(Serializer.Json.SerializeToNode(resource)!);
    }

    /// <summary>
    /// Queries resources of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of resource to query</typeparam>
    /// <param name="resources">The <see cref="IResourceRepository"/> to query</param>
    /// <param name="group">The API group the resources to query belong to</param>
    /// <param name="version">The version of the API the resources to query belong to</param>
    /// <param name="plural">The plural form of the type of resource to query</param>
    /// <param name="namespace">The namespace the resources to query belong to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/></returns>
    public static IAsyncEnumerable<TResource> OfType<TResource>(this IResourceRepository resources, string group, string version, string plural, string? @namespace = null, CancellationToken cancellationToken = default)
        where TResource : V1Resource
    {
        if(string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if(string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if(string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return resources.ListResourcesAsync(group, version, plural, @namespace, cancellationToken: cancellationToken)
            .Select(r => Serializer.Json.Deserialize<TResource>(Serializer.Json.SerializeToNode(r)!)!);
    }

}
