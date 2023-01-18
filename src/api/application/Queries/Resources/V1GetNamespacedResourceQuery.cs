using Microsoft.AspNetCore.Mvc;
using Hylo.Api.Commands;

namespace Hylo.Api.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery"/> used to get a global <see cref="V1Resource"/>
/// </summary>
public class V1GetNamespacedResourceQuery
    : ApiQuery<object>
{

    /// <summary>
    /// Initializes a new <see cref="V1GetNamespacedResourceQuery"/>
    /// </summary>
    public V1GetNamespacedResourceQuery() { }

    /// <summary>
    /// Initializes a new <see cref="V1GetNamespacedResourceQuery"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> to get belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> to get belongs to</param>
    /// <param name="plural">The version of the API the <see cref="V1Resource"/> to get belongs to</param>
    /// <param name="name">The name of the <see cref="V1Resource"/> to get</param>
    /// <param name="namespace">The namespace of the <see cref="V1Resource"/> to get</param>
    public V1GetNamespacedResourceQuery(string group, string version, string plural, string name, string @namespace)
    {
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/> to get belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> to get belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> to get belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets the name of the <see cref="V1Resource"/> to get
    /// </summary>
    [FromRoute, JsonPropertyName("name"), Required]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets the namespace of the <see cref="V1Resource"/> to get
    /// </summary>
    [FromRoute, JsonPropertyName("namespace"), Required]
    public virtual string Namespace { get; set; } = null!;

}

/// <summary>
/// Represents the service used to handle <see cref="V1GetNamespacedResourceQuery"/> instances
/// </summary>
public class V1GetNamespacedResourceQueryHandler
    : ResourceQueryHandlerBase,
    IQueryHandler<V1GetNamespacedResourceQuery, object>
{

    /// <inheritdoc/>
    public V1GetNamespacedResourceQueryHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus eventBus) : base(loggerFactory, resources, versionControl, eventBus) { }

    /// <inheritdoc/>
    public virtual async Task<IResponse<object>> Handle(V1GetNamespacedResourceQuery query, CancellationToken cancellationToken)
    {
        var resource = await this.Resources.GetResourceAsync(query.Group, query.Version, query.Plural, query.Name, query.Namespace, cancellationToken: cancellationToken);
        if (resource == null) return ApiResponse.NamespacedResourceNotFound(query.Group, query.Version, query.Plural, query.Name, query.Namespace);
        return this.Ok(resource);
    }

}
