﻿namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to store <see cref="IResource"/>s
/// </summary>
public interface IResourceStorage
    : IDisposable, IAsyncDisposable
{

    /// <summary>
    /// Inserts the specified <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to insert</param>
    /// <param name="group">The API group the <see cref="IResource"/> to insert belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to insert</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to insert</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to insert belongs to, if any</param>
    /// <param name="subResource">The sub resource to write, if any</param>
    /// <param name="ifNotExists">A boolean indicating whether or not to write the resource only if it does not exist</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The inserted <see cref="IResource"/></returns>
    Task<IResource> WriteAsync(IResource resource, string group, string version, string plural, string? @namespace = null, string? subResource = null, bool ifNotExists = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the <see cref="IResource"/> with the specified name, if any
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/> to read belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to read</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to read</param>
    /// <param name="name">The name of the <see cref="IResource"/> to read</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to read belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IResource"/> with the specified name, if any</returns>
    Task<IResource?> ReadOneAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/>s to list belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/>s to list</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/>s to list</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to list belongs to, if any. If not set, lists resources accross all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resources, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResource"/>s of type specified type</returns>
    Task<ICollection> ReadAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists <see cref="IResource"/>s of the specified type asynchronously
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/>s to stream belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/>s to stream</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/>s to stream</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to stream by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to stream all matching <see cref="IResource"/>s of type specified type</returns>
    IAsyncEnumerable<IResource> ReadAllAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Watches <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/>s to watch belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/>s to watch</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/>s to watch</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to watch belongs to, if any</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to observe by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    Task<IResourceWatch> WatchAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified <see cref="IResource"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/> to delete belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to delete</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to delete</param>
    /// <param name="name">The name of the <see cref="IResource"/> to delete</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to delete belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    Task<IResource> DeleteAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default);

}