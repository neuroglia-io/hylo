using Microsoft.AspNetCore.Mvc;


namespace Hylo.Api.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery"/> used to get a cluster <see cref="V1Resource"/>
/// </summary>
[DataContract]
public class V1GetClusterResourceQuery
    : ApiQuery<object>
{

    /// <summary>
    /// Initializes a new <see cref="V1GetClusterResourceQuery"/>
    /// </summary>
    public V1GetClusterResourceQuery() { }

    /// <summary>
    /// Initializes a new <see cref="V1GetClusterResourceQuery"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> to get belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> to get belongs to</param>
    /// <param name="plural">The version of the API the <see cref="V1Resource"/> to get belongs to</param>
    /// <param name="name">The name of the <see cref="V1Resource"/> to get</param>
    public V1GetClusterResourceQuery(string group, string version, string plural, string name)
    {
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Name = name;
    }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/> to get belongs to
    /// </summary>
    [FromRoute, DataMember(Name = "group", Order = 1), JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> to get belongs to
    /// </summary>
    [FromRoute, DataMember(Name = "version", Order = 2), JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> to get belongs to
    /// </summary>
    [FromRoute, DataMember(Name = "plural", Order = 3), JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets the name of the <see cref="V1Resource"/> to get
    /// </summary>
    [FromRoute, DataMember(Name = "name", Order = 4), JsonPropertyName("name"), Required]
    public virtual string Name { get; set; } = null!;

}

/// <summary>
/// Represents the service used to handle <see cref="V1GetClusterResourceQuery"/> instances
/// </summary>
public class V1GetClusterResourceQueryHandler
    : ResourceQueryHandlerBase,
    IQueryHandler<V1GetClusterResourceQuery, object>
{

    /// <inheritdoc/>
    public V1GetClusterResourceQueryHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus eventBus) : base(loggerFactory, resources, versionControl, eventBus) { }

    /// <inheritdoc/>
    public virtual async Task<IResponse<object>> Handle(V1GetClusterResourceQuery query, CancellationToken cancellationToken)
    {
        var resource = await this.Resources.GetResourceAsync(query.Group, query.Version, query.Plural, query.Name, cancellationToken: cancellationToken);
        if (resource == null) return ApiResponse.ClusterResourceNotFound(query.Group, query.Version, query.Plural, query.Name);
        return this.Ok(resource);
    }

}
