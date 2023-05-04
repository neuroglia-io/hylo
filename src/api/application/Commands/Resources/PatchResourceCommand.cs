namespace Hylo.Api.Application.Commands.Resources;

/// <summary>
/// Represents the <see cref="ICommand"/> used to patch an existing <see cref="IResource"/>
/// </summary>
public class PatchResourceCommand
    : ICommand<IResource>
{

    /// <summary>
    /// Initializes a new <see cref="PatchResourceCommand"/>
    /// </summary>
    /// <param name="group">The API group the resource to patch belongs to</param>
    /// <param name="version">The version of the resource to patch</param>
    /// <param name="plural">The plural name of the type of resource to patch</param>
    /// <param name="name">The name of the <see cref="IResource"/> to patch</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch belongs to</param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changes</param>
    public PatchResourceCommand(string group, string version, string plural, string name, string? @namespace, Patch patch, bool dryRun)
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
        this.Patch = patch ?? throw new ArgumentNullException(nameof(patch));
        this.DryRun = dryRun;
    }

    /// <summary>
    /// Gets the API group the resource to patch belongs to
    /// </summary>
    public string Group { get; }

    /// <summary>
    /// Gets the version of the resource to patch
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the plural name of the type of resource to patch
    /// </summary>
    public string Plural { get; }

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
/// Represents the service used to handle <see cref="PatchResourceCommand"/>s
/// </summary>
public class PatchResourceCommandHandler
    : ResourceRequestHandlerBase, ICommandHandler<PatchResourceCommand, IResource>
{

    /// <inheritdoc/>
    public PatchResourceCommandHandler(IRepository repository) : base(repository) { }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<IResource>> Handle(PatchResourceCommand command, CancellationToken cancellationToken)
    {
        return this.Ok(await this.Repository.PatchAsync(command.Patch, command.Group, command.Version, command.Plural, command.Name, command.Namespace, command.DryRun, cancellationToken).ConfigureAwait(false));
    }

}
