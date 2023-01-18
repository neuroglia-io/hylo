using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hylo.Api.Commands.Resources;

/// <summary>
/// Represents the <see cref="ICommand"/> used to patch a namespaced <see cref="V1Resource"/>
/// </summary>
public class V1PatchNamespacedResourceCommand
    : V1ApiCommand<object>
{

    /// <summary>
    /// Initializes a new <see cref="V1PatchNamespacedResourceCommand"/>
    /// </summary>
    public V1PatchNamespacedResourceCommand() { }

    /// <summary>
    /// Initializes a new <see cref="V1PatchNamespacedResourceCommand"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> belongs to</param>
    /// <param name="plural">The plural name of the type of the <see cref="V1Resource"/> to patch</param>
    /// <param name="name">The name of the resource to patch</param>
    /// <param name="namespace">The namespace the <see cref="V1Resource"/> to operate on belongs to</param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="commandOptions">An object used to configure the <see cref="ResourceCommand"/></param>
    public V1PatchNamespacedResourceCommand(string group, string version, string plural, string name, string @namespace, V1Patch patch, ResourceCommandOptions? commandOptions = null) 
    {
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Name = name;
        this.Namespace = @namespace;
        this.Patch = patch;
        this.CommandOptions = commandOptions;
    }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/> belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("group"), Required]
    public string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("version"), Required]
    public string Version { get; set; } = null!;

    /// <summary>
    /// Gets the plural name of the type of the <see cref="V1Resource"/> to operate on
    /// </summary>
    [FromRoute, JsonPropertyName("plural"), Required]
    public string Plural { get; set; } = null!;

    /// <summary>
    /// Gets the name of the <see cref="V1Resource"/> to operate on
    /// </summary>
    [FromRoute, JsonPropertyName("name"), Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets the namespace the <see cref="V1Resource"/> to operate on belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("namespace"), Required]
    public string Namespace { get; set; } = null!;

    /// <summary>
    /// Gets the patch to apply
    /// </summary>
    [FromBody, JsonPropertyName("patch"), Required]
    public V1Patch Patch { get; set; } = null!;

    /// <summary>
    /// Gets an object used to configure the <see cref="ResourceCommand"/>
    /// </summary>
    [FromQuery, JsonPropertyName("commandOptions")]
    public ResourceCommandOptions? CommandOptions { get; set; }

}

/// <summary>
/// Represents the service used to handle <see cref="V1PatchNamespacedResourceCommand"/>s
/// </summary>
public class V1PatchNamespacedResourceCommandHandler
    : RequestHandlerBase,
    ICommandHandler<V1PatchNamespacedResourceCommand, object>
{

    /// <summary>
    /// Initializes a new <see cref="V1PatchNamespacedResourceCommandHandler"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resources">The services used to manage <see cref="V1Resource"/>s</param>
    /// <param name="mediator">The service used to mediate calls</param>
    public V1PatchNamespacedResourceCommandHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IMediator mediator) 
        : base(loggerFactory) 
    {
        this.Resources = resources;
        this.Mediator = mediator;
    }

    /// <summary>
    /// Gets the services used to manage <see cref="V1Resource"/>s
    /// </summary>
    protected IResourceRepository Resources { get; }

    /// <summary>
    /// Gets the service used to mediate calls
    /// </summary>
    protected IMediator Mediator { get; }

    /// <inheritdoc/>
    public virtual async Task<IResponse<object>> Handle(V1PatchNamespacedResourceCommand command, CancellationToken cancellationToken)
    {
        var resource = (await this.Resources.GetResourceAsync(command.Group, command.Version, command.Plural, command.Name, command.Namespace, cancellationToken: cancellationToken))?.AsJsonObject();
        if (resource == null) return ApiResponse.NamespacedResourceNotFound(command.Group, command.Version, command.Plural, command.Name, command.Namespace);

        resource = command.Patch.ApplyTo(resource);

        return await this.Mediator.Send(new V1UpdateResourceCommand(command.Group, command.Version, command.Plural, command.Name, command.Namespace, resource, command.CommandOptions), cancellationToken);
    }

}