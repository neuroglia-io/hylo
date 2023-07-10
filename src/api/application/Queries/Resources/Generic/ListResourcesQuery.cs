namespace Hylo.Api.Application.Queries.Resources.Generic;

/// <summary>
/// Represents the <see cref="IQuery{TResult}"/> used to list all <see cref="IResource"/>s
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/>s to list</typeparam>
public class ListResourcesQuery<TResource>
    : IQuery<ICollection<TResource>>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="ListResourcesQuery{TResource}"/>
    /// </summary>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to get belong to, if any</param>
    /// <param name="labelSelectors">A <see cref="List{T}"/> containing the <see cref="LabelSelector"/>s used to filter <see cref="IResource"/>s by</param>
    /// <param name="maxResults">The maximum amount of results to list</param>
    /// <param name="continuationToken">The token, if any, used to continue enumerating the results</param>
    public ListResourcesQuery(string? @namespace, IEnumerable<LabelSelector>? labelSelectors, ulong? maxResults, string? continuationToken)
    {
        this.Namespace = @namespace;
        this.LabelSelectors = labelSelectors;
        this.MaxResults = maxResults;
        this.ContinuationToken = continuationToken;
    }

    /// <summary>
    /// Gets the namespace the <see cref="IResource"/>s to get belong to, if any
    /// </summary>
    public string? Namespace { get; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the <see cref="LabelSelector"/>s used to filter <see cref="IResource"/>s by
    /// </summary>
    public IEnumerable<LabelSelector>? LabelSelectors { get; }

    /// <summary>
    /// Gets the maximum amount of results to list
    /// </summary>
    public ulong? MaxResults { get; }

    /// <summary>
    /// Gets the token, if any, used to continue enumerating the results
    /// </summary>
    public string? ContinuationToken { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="ListResourcesQuery{TResource}"/> instances
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/>s to list</typeparam>
public class ListResourcesQueryHandler<TResource>
    : ResourceRequestHandlerBase, IQueryHandler<ListResourcesQuery<TResource>, Hylo.ICollection<TResource>>
    where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public ListResourcesQueryHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<Hylo.ICollection<TResource>>> Handle(ListResourcesQuery<TResource> query, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.ListAsync<TResource>(query.Namespace, query.LabelSelectors, query.MaxResults, query.ContinuationToken, cancellationToken).ConfigureAwait(false));
    }

}
