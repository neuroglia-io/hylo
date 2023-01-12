namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage <see cref="V1Resource"/>s
/// </summary>
public interface IResourceRepository
{

    /// <summary>
    /// Adds the specified resource
    /// </summary>
    /// <param name="group">The API group the resource to add belongs to</param>
    /// <param name="version">The version of the API the resource to add belongs to</param>
    /// <param name="plural">The plural name of the resource to add</param>
    /// <param name="resource">The resource to add</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly added resource</returns>
    Task<object> AddAsync(string group, string version, string plural, object resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether or not the resource with the specified name and namespace exists
    /// </summary>
    /// <param name="group">The API group the resource to check belongs to</param>
    /// <param name="version">The version of the API the resource to check belongs to</param>
    /// <param name="plural">The plural name of the resource to check</param>
    /// <param name="name">The name of the resource to check</param>
    /// <param name="namespace">The namespace the resource belongs to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the resource exists</returns>
    Task<bool> ContainsAsync(string group, string version, string plural, string name, string? @namespace, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the specified <see cref="V1ResourceDefinition"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> to get the <see cref="V1ResourceDefinition"/> for belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> to get the <see cref="V1ResourceDefinition"/> for belongs to</param>
    /// <param name="plural">The plural form of the <see cref="V1Resource"/>'s type to get the <see cref="V1ResourceDefinition"/> for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="V1ResourceDefinition"/> with the specified group, version and plural name</returns>
    Task<V1ResourceDefinition?> GetDefinitionAsync(string group, string version, string plural, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the resource with the specified name and namespace
    /// </summary>
    /// <param name="group">The API group the resource to get belongs to</param>
    /// <param name="version">The version of the API the resource to get belongs to</param>
    /// <param name="plural">The plural name of the resource to get</param>
    /// <param name="name">The name of the resource to get</param>
    /// <param name="namespace">The namespace the resource belongs to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource with the specified name and namespace, if any</returns>
    Task<object?> FindAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists resources of the specified type
    /// </summary>
    /// <param name="group">The API group the resources to get belongs to</param>
    /// <param name="version">The version of the API the resources to get belongs to</param>
    /// <param name="plural">The plural name of the resources to get</param>
    /// <param name="namespace">The namespace to filter resources by</param>
    /// <param name="labelSelectors">An <see cref="IEnumerable{T}"/> containing the label selectors to filter resources by</param>
    /// <param name="resultsPerPage">The amount of results per page</param>
    /// <param name="orderBy">The path to the metadata property to order resources by</param>
    /// <param name="orderByDescending">A boolean indicating whether or not to sort resources in a descending order</param>
    /// <param name="pageIndex">The current page index, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing matched resources</returns>
    IAsyncEnumerable<object> ListAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<string>? labelSelectors = null,
        int resultsPerPage = V1CoreApiDefaults.Paging.MaxResultsPerPage, string? orderBy = null, bool orderByDescending = false, int? pageIndex = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified resource
    /// </summary>
    /// <param name="group">The API group the resource to update belongs to</param>
    /// <param name="version">The version of the API the resource to update belongs to</param>
    /// <param name="plural">The plural name of the resource to update</param>
    /// <param name="resource">The resource to update</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The updated resource</returns>
    Task<object> UpdateAsync(string group, string version, string plural, string name, string? @namespace, object resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified resource
    /// </summary>
    /// <param name="group">The API group the resource to remove belongs to</param>
    /// <param name="version">The version of the API the resource to remove belongs to</param>
    /// <param name="plural">The plural name of the resource to remove</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The removed resource</returns>
    Task<object> RemoveAsync(string group, string version, string plural, string name, string? @namespace, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);

}