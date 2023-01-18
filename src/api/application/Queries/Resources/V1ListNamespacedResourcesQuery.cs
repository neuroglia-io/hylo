using Microsoft.AspNetCore.Mvc;

namespace Hylo.Api.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery"/> used to list <see cref="V1ResourceEvent"/>s of a specified namespaced <see cref="V1Resource"/> type
/// </summary>
public class V1ListNamespacedResourcesQuery
    : ApiQuery<IAsyncEnumerable<object>>
{

    /// <summary>
    /// Initializes a new <see cref="V1ListNamespacedResourcesQuery"/>
    /// </summary>
    public V1ListNamespacedResourcesQuery() { }

    /// <summary>
    /// Initializes a new <see cref="V1ListNamespacedResourcesQuery"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/>s to list belong to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/>s to list belong to</param>
    /// <param name="plural">The version of the API the <see cref="V1Resource"/>s to list belong to</param>
    /// <param name="namespace">The namespace the <see cref="V1Resource"/>s to list belong to</param>
    /// <param name="labelSelectors">The comma-separated label selectors to filter by the resources to list</param>
    /// <param name="orderBy">The path to the metadata property to order by the <see cref="V1Resource"/>s to list</param>
    /// <param name="orderByDescending">A boolean indicating whether or not the sort the <see cref="V1Resource"/> in a descending order</param>
    /// <param name="resultsPerPage">The amount of results per page</param>
    /// <param name="continuationToken">The query's continuation token, used to resume result enumeration from a previous query, if any</param>
    /// <param name="pageIndex">The current page index, if any</param>
    public V1ListNamespacedResourcesQuery(string group, string version, string plural, string @namespace, string? labelSelectors = null, string? orderBy = null, bool orderByDescending = false, int resultsPerPage = V1CoreApiDefaults.Paging.MaxResultsPerPage, string? continuationToken = null, int? pageIndex = null)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));
        if (resultsPerPage < V1CoreApiDefaults.Paging.MinResultsPerPage || resultsPerPage > V1CoreApiDefaults.Paging.MaxResultsPerPage) throw new ArgumentOutOfRangeException(nameof(resultsPerPage));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.LabelSelectors = labelSelectors;
        this.OrderBy = orderBy;
        this.OrderByDescending = orderByDescending;
        this.ResultsPerPage = resultsPerPage;
        this.ContinuationToken = continuationToken;
        this.PageIndex = pageIndex;
    }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/>s to list belong to
    /// </summary>
    [FromRoute, JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/>s to list belong to
    /// </summary>
    [FromRoute, JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/>s to list belong to
    /// </summary>
    [FromRoute, JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets the namespace the <see cref="V1Resource"/>s to list belong to
    /// </summary>
    [FromRoute, JsonPropertyName("namespace"), Required]
    public virtual string Namespace { get; set; } = null!;

    /// <summary>
    /// Gets the path to the metadata property to order by the <see cref="V1Resource"/>s to list
    /// </summary>
    [FromQuery, JsonPropertyName("orderBy")]
    public virtual string? OrderBy { get; set; }

    /// <summary>
    /// Gets a boolean indicating whether or not the sort the <see cref="V1Resource"/> in a descending order
    /// </summary>
    [FromQuery, JsonPropertyName("orderByDescending")]
    public virtual bool OrderByDescending { get; set; }

    /// <summary>
    /// Gets the comma-separated label selectors to filter by the resources to list
    /// </summary>
    [FromQuery, JsonPropertyName("labelSelectors")]
    public virtual string? LabelSelectors { get; set; }

    /// <summary>
    /// Gets the amount of results per page.<para></para>
    /// <see cref="MinResultsPerPage">Minimum</see><para></para>
    /// <see cref="MaxResultsPerPage">Maximum</see>
    /// </summary>
    [FromQuery, JsonPropertyName("resultsPerPage"), Range(V1CoreApiDefaults.Paging.MinResultsPerPage, V1CoreApiDefaults.Paging.MaxResultsPerPage)]
    public virtual int ResultsPerPage { get; set; } = V1CoreApiDefaults.Paging.MaxResultsPerPage;

    /// <summary>
    /// Gets the query's continuation token, used to resume result enumeration from a previous query
    /// </summary>
    [FromQuery, JsonPropertyName("continuationToken")]
    public virtual string? ContinuationToken { get; set; }

    /// <summary>
    /// Gets the current page index, if any
    /// </summary>
    [FromQuery, JsonPropertyName("pageIndex")]
    public virtual int? PageIndex { get; set; }

}

/// <summary>
/// Represents the service used to handle <see cref="V1ListNamespacedResourcesQuery"/> instances
/// </summary>
public class V1ListNamespacedResourcesQueryHandler
    : ResourceQueryHandlerBase,
    IQueryHandler<V1ListNamespacedResourcesQuery, IAsyncEnumerable<object>>
{

    /// <inheritdoc/>
    public V1ListNamespacedResourcesQueryHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus eventBus) : base(loggerFactory, resources, versionControl, eventBus) { }

    /// <inheritdoc/>
    public virtual async Task<IResponse<IAsyncEnumerable<object>>> Handle(V1ListNamespacedResourcesQuery query, CancellationToken cancellationToken)
    {
        var labelSelectors = query.LabelSelectors?.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var resources = this.Resources.ListResourcesAsync(query.Group, query.Version, query.Plural, query.Namespace, labelSelectors, query.ResultsPerPage, query.OrderBy, query.OrderByDescending, query.PageIndex, cancellationToken);
        return await Task.FromResult(this.Ok(resources));
    }

}