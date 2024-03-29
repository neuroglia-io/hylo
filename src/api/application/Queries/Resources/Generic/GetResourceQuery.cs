﻿namespace Hylo.Api.Application.Queries.Resources.Generic;

/// <summary>
/// Represents the <see cref="IQuery{TResult}"/> used to get an existing <see cref="IResource"/>
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to get</typeparam>
public class GetResourceQuery<TResource>
    : IQuery<TResource>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="GetResourceQuery{TResource}"/>
    /// </summary>
    /// <param name="name">The name of the <see cref="IResource"/> to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to get belongs to</param>
    public GetResourceQuery(string name, string? @namespace)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Name = name;
        this.Namespace = @namespace;
    }

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
/// Represents the service used to handle <see cref="GetResourceQuery{TResource}"/>s
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace</typeparam>
public class GetResourceQueryHandler<TResource>
    : ResourceRequestHandlerBase, IQueryHandler<GetResourceQuery<TResource>, TResource>
     where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public GetResourceQueryHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TResource>> Handle(GetResourceQuery<TResource> query, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.GetAsync<TResource>(query.Name, query.Namespace, cancellationToken).ConfigureAwait(false));
    }

}
