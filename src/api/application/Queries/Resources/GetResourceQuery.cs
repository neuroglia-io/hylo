namespace Hylo.Api.Application.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery{TResult}"/> used to get an existing <see cref="IResource"/>
/// </summary>
public class GetResourceQuery
    : IQuery<IResource>
{

    /// <summary>
    /// Initializes a new <see cref="GetResourceQuery"/>
    /// </summary>
    /// <param name="group">The API group the resource to get belongs to</param>
    /// <param name="version">The version of the resource to get</param>
    /// <param name="plural">The plural name of the type of resource to get</param>
    /// <param name="name">The name of the <see cref="IResource"/> to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to get belongs to</param>
    public GetResourceQuery(string group, string version, string plural, string name, string? @namespace)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <summary>
    /// Gets the API group the resource to get belongs to
    /// </summary>
    public string Group { get; }

    /// <summary>
    /// Gets the version of the resource to get
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the plural name of the type of resource to get
    /// </summary>
    public string Plural { get; }

    /// <summary>
    /// Gets the name of the <see cref="IResource"/> to get
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the namespace the <see cref="IResource"/> to get belongs to
    /// </summary>
    public string? Namespace { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="GetResourceQuery"/>s
/// </summary>
public class GetResourceQueryHandler
    : ResourceRequestHandlerBase, IQueryHandler<GetResourceQuery, IResource>
{

    /// <inheritdoc/>
    public GetResourceQueryHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<IResource>> Handle(GetResourceQuery query, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.GetAsync(query.Group, query.Version, query.Plural, query.Name, query.Namespace, cancellationToken).ConfigureAwait(false));
    }

}
