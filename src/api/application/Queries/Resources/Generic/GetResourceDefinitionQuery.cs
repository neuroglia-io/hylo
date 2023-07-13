namespace Hylo.Api.Application.Queries.Resources.Generic;

/// <summary>
/// Represents the <see cref="IQuery{TResult}"/> used to get the definition of the specified <see cref="IResource"/> type
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to get the definition of</typeparam>
public class GetResourceDefinitionQuery<TResource>
    : IQuery<IResourceDefinition>
    where TResource : class, IResource, new()
{



}

/// <summary>
/// Represents the service used to handle <see cref="GetResourceDefinitionQuery{TResource}"/>s
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace</typeparam>
public class GetResourceDefinitionQueryHandler<TResource>
    : ResourceRequestHandlerBase, IQueryHandler<GetResourceDefinitionQuery<TResource>, IResourceDefinition>
     where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public GetResourceDefinitionQueryHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<IResourceDefinition>> Handle(GetResourceDefinitionQuery<TResource> query, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.GetDefinitionAsync<TResource>(cancellationToken).ConfigureAwait(false));
    }

}