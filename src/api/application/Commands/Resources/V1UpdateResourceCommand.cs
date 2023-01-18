using Microsoft.AspNetCore.Mvc;

namespace Hylo.Api.Commands.Resources;

/// <summary>
/// Represents the <see cref="ResourceCommand"/> used to update a <see cref="V1Resource"/>
/// </summary>
public class V1UpdateResourceCommand
    : ResourceCommand
{

    /// <summary>
    /// Initializes a new <see cref="V1UpdateResourceCommand"/>
    /// </summary>
    public V1UpdateResourceCommand() { }

    /// <summary>
    /// Initializes a new <see cref="V1UpdateResourceCommand"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> belongs to</param>
    /// <param name="plural">The plural name of the type of the <see cref="V1Resource"/> to update</param>
    /// <param name="name">The name of the resource to update</param>
    /// <param name="namespace">The namespace the resource to update belongs to</param>
    /// <param name="resource">The state of the resource to update</param>
    /// <param name="commandOptions">An object used to configure the <see cref="ResourceCommand"/></param>
    public V1UpdateResourceCommand(string group, string version, string plural, string name, string? @namespace, object resource, ResourceCommandOptions? commandOptions = null) 
        : base(group, version, plural, resource, commandOptions)
    {
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <inheritdoc/>
    public override string Operation => V1ApiVerb.Update;

    /// <summary>
    /// Gets the name of the resource to update
    /// </summary>
    [FromRoute, JsonPropertyName("name"), Required]
    public virtual string Name { get; protected set; } = null!;

    /// <summary>
    /// Gets the namespace the resource to update belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("namespace"), Required]
    public virtual string? Namespace { get; protected set; } = null!;

    /// <inheritdoc/>
    public override V1ResourceReference GetResourceReference() => new(this.Group, this.Version, this.Plural, this.Name);

}

/// <summary>
/// Represents the service used to handle <see cref="V1UpdateResourceCommand"/>s
/// </summary>
public class V1UpdateResourceCommandHandler
    : ResourceCommandHandlerBase<V1UpdateResourceCommand>
{

    /// <inheritdoc/>
    public V1UpdateResourceCommandHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus resourceEventBus) 
        : base(loggerFactory, resources, versionControl, resourceEventBus)
    {
    }

    /// <inheritdoc/>
    public override async Task<IResponse<object>> Handle(V1UpdateResourceCommand command, CancellationToken cancellationToken)
    {
        var resourceDefinition = await this.Resources.GetResourceDefinitionAsync(command.Group, command.Version, command.Plural, cancellationToken);
        if (resourceDefinition == null) return ApiResponse.ResourceDefinitionNotFound<object>(command.Group, command.Version, command.Plural);
        var resource = command.Resource.AsJsonObject();
        if (resource == null) return ApiResponse.InvalidResourceFormat();

        var resourceApiVersion = resource.GetResourceApiVersion();
        var resourceKind = resource.GetResourceKind();
        if (!ApiVersion.Build(command.Group, command.Version).Equals(resourceApiVersion, StringComparison.OrdinalIgnoreCase) || !resourceDefinition.Spec.Names.Kind.Equals(resourceKind, StringComparison.OrdinalIgnoreCase)) return ApiResponse.WrongApi();

        var storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new(new(command.Group, command.Version, command.Plural), resourceDefinition, resource), cancellationToken);
        await this.Resources.UpdateResourceAsync(command.Group, command.Version, command.Plural, command.Name, command.Namespace, storageResource, cancellationToken);
        if (command.CommandOptions == null || command.CommandOptions.DryRun == false) await this.Resources.SaveChangesAsync(cancellationToken);

        await this.ResourceEventBus.PublishAsync(new(V1ResourceEventType.Updated, command.Group, command.Version, command.Plural, command.Resource), cancellationToken);

        return this.Created(resource);
    }

}
