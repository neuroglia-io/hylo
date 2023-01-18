using Microsoft.AspNetCore.Mvc;
using System.Reactive.Linq;

namespace Hylo.Api.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery"/> used to watch the <see cref="V1ResourceEvent"/>s of a specified cluster <see cref="V1Resource"/> type
/// </summary>
public class V1WatchClusterResourcesQuery
    : ApiQuery<IAsyncEnumerable<V1ResourceEvent>>
{

    /// <summary>
    /// Initializes a new <see cref="V1WatchClusterResourcesQuery"/>
    /// </summary>
    public V1WatchClusterResourcesQuery() { }

    /// <summary>
    /// Initializes a new <see cref="V1WatchClusterResourcesQuery"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/>s to watch belong to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/>s to watch belong to</param>
    /// <param name="plural">The version of the API the <see cref="V1Resource"/>s to watch belong to</param>
    public V1WatchClusterResourcesQuery(string group, string version, string plural)
    {
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
    }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/>s to watch belong to
    /// </summary>
    [FromRoute, JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/>s to watch belong to
    /// </summary>
    [FromRoute, JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/>s to watch belong to
    /// </summary>
    [FromRoute, JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

}

/// <summary>
/// Represents the service used to handle <see cref="V1WatchClusterResourcesQuery"/> instances
/// </summary>
public class V1WatchClusterResourcesQueryHandler
    : ResourceQueryHandlerBase,
    IQueryHandler<V1WatchClusterResourcesQuery, IAsyncEnumerable<V1ResourceEvent>>
{

    /// <inheritdoc/>
    public V1WatchClusterResourcesQueryHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus eventBus) : base(loggerFactory, resources, versionControl, eventBus) { }

    /// <inheritdoc/>
    public virtual async Task<IResponse<IAsyncEnumerable<V1ResourceEvent>>> Handle(V1WatchClusterResourcesQuery query, CancellationToken cancellationToken)
    {
        var events = this.EventBus
            .Where(e => 
                e.Group.Equals(query.Group, StringComparison.OrdinalIgnoreCase) 
                && e.Version.Equals(query.Version, StringComparison.OrdinalIgnoreCase) 
                && e.Plural.Equals(query.Plural, StringComparison.OrdinalIgnoreCase))
            .ToAsyncEnumerable();
        return await Task.FromResult(this.Ok(events));
    }

}
