namespace Hylo.Api.Application.Commands.Resources.Generic;

/// <summary>
/// Represents the <see cref="ICommand"/> used to patch an existing <see cref="IResource"/>
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to patch</typeparam>
public class PatchResourceCommand<TResource>
    : ICommand<TResource>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Initializes a new <see cref="PatchResourceCommand{TResource}"/>
    /// </summary>
    /// <param name="name">The name of the <see cref="IResource"/> to patch</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch belongs to</param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    public PatchResourceCommand(string name, string? @namespace, Patch patch, bool dryRun)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Name = name;
        this.Namespace = @namespace;
        this.Patch = patch ?? throw new ArgumentNullException(nameof(patch));
        this.DryRun = dryRun;
    }

    /// <summary>
    /// Gets the name of the <see cref="IResource"/> to patch
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the name of the <see cref="IResource"/> to patch
    /// </summary>
    public string? Namespace { get; }

    /// <summary>
    /// Gets the patch to apply
    /// </summary>
    public Patch Patch { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not to persist changes
    /// </summary>
    public bool DryRun { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="PatchResourceCommand{TResource}"/>s
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to patch</typeparam>
public class PatchResourceCommandHandler<TResource>
    : ResourceRequestHandlerBase, ICommandHandler<PatchResourceCommand<TResource>, TResource>
    where TResource : class, IResource, new()
{

    /// <inheritdoc/>
    public PatchResourceCommandHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TResource>> Handle(PatchResourceCommand<TResource> command, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.PatchAsync<TResource>(command.Patch, command.Name, command.Namespace, command.DryRun, cancellationToken).ConfigureAwait(false));
    }

}
