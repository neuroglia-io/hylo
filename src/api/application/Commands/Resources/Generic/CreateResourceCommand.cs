namespace Hylo.Api.Application.Commands.Resources.Generic;

/// <summary>
/// Represents the command used to create a new <see cref="IResource"/>
/// </summary>
public class CreateResourceCommand<TResource>
    : ICommand<TResource>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="CreateResourceCommand{TResource}"/>
    /// </summary>
    /// <param name="resource">The resource to create</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induces by the command</param>
    public CreateResourceCommand(TResource resource, bool dryRun)
    {
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        this.DryRun = dryRun;
    }

    /// <summary>
    /// Gets the resource to create
    /// </summary>
    public TResource Resource { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not to persist the changes induces by the command
    /// </summary>
    public bool DryRun { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="CreateResourceCommand{TResource}"/>s
/// </summary>
public class CreateResourceCommandHandler<TResource>
    : ResourceRequestHandlerBase, ICommandHandler<CreateResourceCommand<TResource>, TResource>
    where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public CreateResourceCommandHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TResource>> Handle(CreateResourceCommand<TResource> command, CancellationToken cancellationToken)
    {
        return this.Created(await this.Repository.AddAsync(command.Resource, command.DryRun, cancellationToken).ConfigureAwait(false));
    }

}
