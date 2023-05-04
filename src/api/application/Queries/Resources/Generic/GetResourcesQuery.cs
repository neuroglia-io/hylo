namespace Hylo.Api.Application.Queries.Resources.Generic;

/// <summary>
/// Represents the <see cref="IQuery{TResult}"/> used to retrieves <see cref="IResource"/>s of the specified type
/// </summary>
public class GetResourcesQuery<TResource>
    : IQuery<IAsyncEnumerable<TResource>>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="GetResourcesQuery{TResource}"/>
    /// </summary>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to get belong to, if any</param>
    /// <param name="labelSelectors">A <see cref="List{T}"/> containing the <see cref="LabelSelector"/>s used to filter <see cref="IResource"/>s by</param>
    public GetResourcesQuery(string? @namespace, IEnumerable<LabelSelector>? labelSelectors)
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
/// Represents the service used to handle <see cref="GetResourcesQuery{TResource}"/> instances
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to get</typeparam>
public class GetResourcesQueryHandler<TResource>
    : ResourceRequestHandlerBase, IQueryHandler<GetResourcesQuery<TResource>, IAsyncEnumerable<TResource>>
    where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public GetResourcesQueryHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual Task<ApiResponse<IAsyncEnumerable<TResource>>> Handle(GetResourcesQuery<TResource> query, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.Ok(this.Repository.GetAllAsync<TResource>(query.Namespace, query.LabelSelectors, cancellationToken)));
    }

}