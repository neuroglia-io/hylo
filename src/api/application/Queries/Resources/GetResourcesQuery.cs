namespace Hylo.Api.Application.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery{TResult}"/> used to retrieves <see cref="IResource"/>s of the specified type
/// </summary>
public class GetResourcesQuery
    : IQuery<IAsyncEnumerable<IResource>>
{

    /// <summary>
    /// Initializes a new <see cref="GetResourcesQuery"/>
    /// </summary>
    /// <param name="group">The API group the resources to get belongs to</param>
    /// <param name="version">The version of the resources to get</param>
    /// <param name="plural">The plural name of the type of resources to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to get belong to, if any</param>
    /// <param name="labelSelectors">A <see cref="List{T}"/> containing the <see cref="LabelSelector"/>s used to filter <see cref="IResource"/>s by</param>
    public GetResourcesQuery(string group, string version, string plural, string? @namespace, List<LabelSelector>? labelSelectors)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Namespace = @namespace;
        this.LabelSelectors = labelSelectors;
    }

    /// <summary>
    /// Gets the API group the resources to get belongs to
    /// </summary>
    public string Group { get; }

    /// <summary>
    /// Gets the version of the resources to get
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the plural name of the type of resources to get
    /// </summary>
    public string Plural { get; }

    /// <summary>
    /// Gets the namespace the <see cref="IResource"/>s to get belong to, if any
    /// </summary>
    public string? Namespace { get; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the <see cref="LabelSelector"/>s used to filter <see cref="IResource"/>s by
    /// </summary>
    public List<LabelSelector>? LabelSelectors { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="GetResourcesQuery"/> instances
/// </summary>
public class GetResourcesQueryHandler
    : ResourceRequestHandlerBase, IQueryHandler<GetResourcesQuery, IAsyncEnumerable<IResource>>
{

    /// <inheritdoc/>
    public GetResourcesQueryHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual Task<ApiResponse<IAsyncEnumerable<IResource>>> Handle(GetResourcesQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.Ok(this.Repository.GetAllAsync(query.Group, query.Version, query.Plural, query.Namespace, query.LabelSelectors, cancellationToken)));
    }

}