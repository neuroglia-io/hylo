namespace Hylo.Api.Application.Commands.Resources.Generic;

/// <summary>
/// Represents the <see cref="ICommand"/> used to replace an existing <see cref="IResource"/>
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace</typeparam>
public class ReplaceResourceCommand<TResource>
    : ICommand<TResource>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="ReplaceResourceCommand{TResource}"/>
    /// </summary>
    /// <param name="resource">The updated <see cref="IResource"/> to replace</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    public ReplaceResourceCommand(TResource resource, bool dryRun)
    {
        this.Resource = resource;
        this.DryRun = dryRun;
    }

    /// <summary>
    /// Gets the updated <see cref="IResource"/> to replace
    /// </summary>
    public TResource Resource { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not to persist changes
    /// </summary>
    public bool DryRun { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="ReplaceResourceCommand{TResource}"/>s
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace</typeparam>
public class ReplaceResourceCommandHandler<TResource>
    : ResourceRequestHandlerBase, ICommandHandler<ReplaceResourceCommand<TResource>, TResource>
     where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public ReplaceResourceCommandHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TResource>> Handle(ReplaceResourceCommand<TResource> command, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.ReplaceAsync(command.Resource, command.DryRun, cancellationToken).ConfigureAwait(false));
    }

}
