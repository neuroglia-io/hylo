namespace Hylo.Api.Commands;

/// <summary>
/// Represents the base class for all <see cref="V1Resource"/>-related <see cref="ICommandHandler"/>s
/// </summary>
/// <typeparam name="TCommand">The type of <see cref="ResourceCommand"/> to handle</typeparam>
public abstract class ResourceCommandHandlerBase<TCommand>
    : RequestHandlerBase,
    ICommandHandler<TCommand, object>
    where TCommand : ResourceCommand, new()
{

    /// <summary>
    /// Initializes a new <see cref="ResourceCommandHandlerBase{TCommand}"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="versionControl">The service used to control <see cref="V1Resource"/> versions</param>
    /// <param name="resourceEventBus">The service used to publish and subscribe to <see cref="V1Resource"/> events</param>
    protected ResourceCommandHandlerBase(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus resourceEventBus) 
        : base(loggerFactory)
    {
        this.Resources = resources;
        this.VersionControl = versionControl;
        this.ResourceEventBus = resourceEventBus;
    }

    /// <summary>
    /// Gets the service used to manage <see cref="V1Resource"/>s
    /// </summary>
    protected IResourceRepository Resources { get; }

    /// <summary>
    /// Gets the service used to control <see cref="V1Resource"/> versions
    /// </summary>
    protected IResourceVersionControl VersionControl { get; }

    /// <summary>
    /// Gets the service used to publish and subscribe to <see cref="V1Resource"/> events
    /// </summary>
    protected IResourceEventBus ResourceEventBus { get; }

    /// <inheritdoc/>
    public abstract Task<IResponse<object>> Handle(TCommand command, CancellationToken cancellationToken);

}
