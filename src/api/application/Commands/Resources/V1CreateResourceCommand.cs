namespace Hylo.Api.Commands.Resources;

/// <summary>
/// Represents the <see cref="ICommand"/> used to create a new <see cref="V1Resource"/>
/// </summary>
public class V1CreateResourceCommand
    : ResourceCommand
{

    /// <summary>
    /// Initializes a new <see cref="V1CreateResourceCommand"/>
    /// </summary>
    public V1CreateResourceCommand() { }

    /// <inheritdoc/>
    public V1CreateResourceCommand(string group, string version, string plural, object resource, ResourceCommandOptions? commandOptions = null) : base(group, version, plural, resource, commandOptions) { }

    /// <inheritdoc/>
    public override string Operation => V1ApiVerb.Create;

    /// <inheritdoc/>
    public override V1ResourceReference GetResourceReference() => new(this.Group, this.Version, this.Plural);

}

/// <summary>
/// Represents the service used to handle <see cref="V1CreateResourceCommand"/>s
/// </summary>
public class V1CreateResourceCommandHandler
    : ResourceCommandHandlerBase<V1CreateResourceCommand>
{

    /// <inheritdoc/>
    public V1CreateResourceCommandHandler(ILoggerFactory loggerFactory, IResourceRepository resources, 
        IResourceVersionControl versionControl, IResourceEventBus resourceEventBus) 
        : base(loggerFactory, resources, versionControl, resourceEventBus) { }

    /// <inheritdoc/>
    public override async Task<IResponse<object>> Handle(V1CreateResourceCommand command, CancellationToken cancellationToken)
    {
        var resourceDefinition = await this.Resources.GetResourceDefinitionAsync(command.Group, command.Version, command.Plural, cancellationToken).ConfigureAwait(false);
        if (resourceDefinition == null) return ApiResponse.ResourceDefinitionNotFound<object>(command.Group, command.Version, command.Plural);
        var resource = command.Resource.AsJsonObject();
        if (resource == null) return ApiResponse.InvalidResourceFormat();

        var resourceApiVersion = resource.GetResourceApiVersion();
        var resourceKind = resource.GetResourceKind();
        if (!ApiVersion.Build(command.Group, command.Version).Equals(resourceApiVersion, StringComparison.OrdinalIgnoreCase) || !resourceDefinition.Spec.Names.Kind.Equals(resourceKind, StringComparison.OrdinalIgnoreCase)) return ApiResponse.WrongApi();

        var storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new(new(command.Group, command.Version, command.Plural), resourceDefinition, resource), cancellationToken).ConfigureAwait(false);
        await this.Resources.AddResourceAsync(command.Group, command.Version, command.Plural, storageResource, cancellationToken).ConfigureAwait(false);
        if (command.CommandOptions == null || command.CommandOptions.DryRun == false) await this.Resources.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await this.ResourceEventBus.PublishAsync(new(V1ResourceEventType.Created, command.Group, command.Version, command.Plural, resource), cancellationToken).ConfigureAwait(false);

        return this.Created(resource);
    }

}
