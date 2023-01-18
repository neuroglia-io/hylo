using Microsoft.AspNetCore.Mvc;
using Hylo.Api.Commands;

namespace Hylo.Api.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery"/> used to list <see cref="V1ResourceEvent"/>s of a specified cluster <see cref="V1Resource"/> type
/// </summary>
public class V1ListClusterResourcesQuery
    : ApiQuery<IAsyncEnumerable<object>>
{

    /// <summary>
    /// Initializes a new <see cref="V1ListClusterResourcesQuery"/>
    /// </summary>
    public V1ListClusterResourcesQuery() { }

    /// <summary>
    /// Initializes a new <see cref="V1ListClusterResourcesQuery"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/>s to list belong to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/>s to list belong to</param>
    /// <param name="plural">The version of the API the <see cref="V1Resource"/>s to list belong to</param>
    /// <param name="labelSelectors">The comma-separated label selectors to filter by the resources to list</param>
    /// <param name="orderBy">The path to the metadata property to order by the <see cref="V1Resource"/>s to list</param>
    /// <param name="orderByDescending">A boolean indicating whether or not the sort the <see cref="V1Resource"/> in a descending order</param>
    /// <param name="resultsPerPage">The amount of results per page</param>
    public V1ListClusterResourcesQuery(string group, string version, string plural, string? labelSelectors = null, string? orderBy = null, bool orderByDescending = false, int resultsPerPage = V1CoreApiDefaults.Paging.MaxResultsPerPage, int? pageIndex = null)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (resultsPerPage < V1CoreApiDefaults.Paging.MinResultsPerPage || resultsPerPage > V1CoreApiDefaults.Paging.MaxResultsPerPage) throw new ArgumentOutOfRangeException(nameof(resultsPerPage));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.LabelSelectors = labelSelectors;
        this.OrderBy = orderBy;
        this.OrderByDescending = orderByDescending;
        this.ResultsPerPage = resultsPerPage;
        this.PageIndex = pageIndex;
    }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/>s to list belong to
    /// </summary>
    [FromRoute(Name = "group"), DataMember(Name = "group", Order = 1), JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/>s to list belong to
    /// </summary>
    [FromRoute(Name = "version"), DataMember(Name = "version", Order = 2), JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/>s to list belong to
    /// </summary>
    [FromRoute(Name = "plural"), DataMember(Name = "plural", Order = 3), JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets the path to the metadata property to order by the <see cref="V1Resource"/>s to list
    /// </summary>
    [FromQuery(Name = "orderBy"), DataMember(Name = "orderBy", Order = 4), JsonPropertyName("orderBy")]
    public virtual string? OrderBy { get; set; }

    /// <summary>
    /// Gets a boolean indicating whether or not the sort the <see cref="V1Resource"/> in a descending order
    /// </summary>
    [FromQuery(Name = "orderByDescending"), DataMember(Name = "orderByDescending", Order = 5), JsonPropertyName("orderByDescending")]
    public virtual bool OrderByDescending { get; set; }

    /// <summary>
    /// Gets the comma-separated label selectors to filter by the resources to list
    /// </summary>
    [FromQuery(Name = "labelSelectors"), DataMember(Name = "labelSelectors", Order = 6), JsonPropertyName("labelSelectors")]
    public virtual string? LabelSelectors { get; set; }

    /// <summary>
    /// Gets the amount of results per page.<para></para>
    /// </summary>
    [FromQuery(Name = "resultsPerPage"), DataMember(Name = "resultsPerPage", Order = 7), JsonPropertyName("resultsPerPage"), Range(V1CoreApiDefaults.Paging.MinResultsPerPage, V1CoreApiDefaults.Paging.MaxResultsPerPage)]
    public virtual int ResultsPerPage { get; set; } = V1CoreApiDefaults.Paging.MaxResultsPerPage;

    /// <summary>
    /// Gets the current page index, if any
    /// </summary>
    [FromQuery(Name = "pageIndex"), DataMember(Name = "pageIndex", Order = 8), JsonPropertyName("pageIndex")]
    public virtual int? PageIndex { get; set; }

}

/// <summary>
/// Represents the service used to handle <see cref="V1ListClusterResourcesQuery"/> instances
/// </summary>
public class V1ListClusterResourcesQueryHandler
    : ResourceQueryHandlerBase,
    IQueryHandler<V1ListClusterResourcesQuery, IAsyncEnumerable<object>>
{

    /// <inheritdoc/>
    public V1ListClusterResourcesQueryHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus eventBus) : base(loggerFactory, resources, versionControl, eventBus) { }

    /// <inheritdoc/>
    public virtual async Task<IResponse<IAsyncEnumerable<object>>> Handle(V1ListClusterResourcesQuery query, CancellationToken cancellationToken)
    {
        var labelSelectors = query.LabelSelectors?.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var resources = this.Resources.ListResourcesAsync(query.Group, query.Version, query.Plural, null, labelSelectors, query.ResultsPerPage, query.OrderBy, query.OrderByDescending, query.PageIndex, cancellationToken);
        return await Task.FromResult(this.Ok(resources));
    }

}
