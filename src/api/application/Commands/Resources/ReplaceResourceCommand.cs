namespace Hylo.Api.Application.Commands.Resources;

/// <summary>
/// Represents the <see cref="ICommand"/> used to replace an existing <see cref="IResource"/>
/// </summary>
public class ReplaceResourceCommand
    : ICommand<IResource>
{

    /// <summary>
    /// Initializes a new <see cref="ReplaceResourceCommand"/>
    /// </summary>
    /// <param name="group">The API group the resource to replace belongs to</param>
    /// <param name="version">The version of the resource to replace</param>
    /// <param name="plural">The plural name of the type of resource to replace</param>
    /// <param name="resource">The updated <see cref="IResource"/> to replace</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    public ReplaceResourceCommand(string group, string version, string plural, IResource resource, bool dryRun)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Resource = resource;
        this.DryRun = dryRun;
    }

    /// <summary>
    /// Gets the API group the resource to replace belongs to
    /// </summary>
    public string Group { get; }

    /// <summary>
    /// Gets the version of the resource to replace
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the plural name of the type of resource to replace
    /// </summary>
    public string Plural { get; }

    /// <summary>
    /// Gets the updated <see cref="IResource"/> to replace
    /// </summary>
    public IResource Resource { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not to persist changes
    /// </summary>
    public bool DryRun { get; }

}

/// <summary>
/// Represents the service used to handle <see cref="ReplaceResourceCommand"/>s
/// </summary>
public class ReplaceResourceCommandHandler
    : ResourceRequestHandlerBase, ICommandHandler<ReplaceResourceCommand, IResource>
{

    /// <inheritdoc/>
    public ReplaceResourceCommandHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<IResource>> Handle(ReplaceResourceCommand command, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.ReplaceAsync(command.Resource, command.Group, command.Version, command.Plural, command.DryRun, cancellationToken).ConfigureAwait(false));
    }

}
