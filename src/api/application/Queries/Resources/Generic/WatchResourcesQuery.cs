namespace Hylo.Api.Application.Queries.Resources.Generic;

/// <summary>
/// Represents the <see cref="IQuery{TResult}"/> used to retrieves <see cref="IResource"/>s of the specified type
/// </summary>
public class WatchResourcesQuery<TResource>
    : IQuery<IResourceWatch<TResource>>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="WatchResourcesQuery{TResource}"/>
    /// </summary>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to get belong to, if any</param>
    /// <param name="labelSelectors">A <see cref="List{T}"/> containing the <see cref="LabelSelector"/>s used to filter <see cref="IResource"/>s by</param>
    public WatchResourcesQuery(string? @namespace, IEnumerable<LabelSelector>? labelSelectors)
    {
        this.Namespace = @namespace;
        this.LabelSelectors = labelSelectors;
    }

    /// <summary>
    /// Gets the namespace the <see cref="IResource"/>s to get belong to, if any
    /// </summary>
    public string? Namespace { get; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the <see cref="LabelSelector"/>s used to filter <see cref="IResource"/>s by
    /// </summary>
    public IEnumerable<LabelSelector>? LabelSelectors { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="WatchResourcesQuery{TResource}"/> instances
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to get</typeparam>
public class WatchResourcesQueryHandler<TResource>
    : ResourceRequestHandlerBase, IQueryHandler<WatchResourcesQuery<TResource>, IResourceWatch<TResource>>
    where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public WatchResourcesQueryHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<IResourceWatch<TResource>>> Handle(WatchResourcesQuery<TResource> query, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.WatchAsync<TResource>(query.Namespace, query.LabelSelectors, cancellationToken).ConfigureAwait(false));
    }

}