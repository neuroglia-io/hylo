namespace Hylo.Api.Queries;

/// <summary>
/// Represents the base class for all <see cref="V1Resource"/>-related <see cref="ICommandHandler"/>s
/// </summary>
public abstract class ResourceQueryHandlerBase
    : RequestHandlerBase
{

    /// <summary>
    /// Initializes a new <see cref="ResourceQueryHandlerBase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resources">The service used to manage <see cref="V1Resource"/>s</param>
    /// <param name="versionControl">The service used to control <see cref="V1Resource"/> versions</param>
    /// <param name="eventBus">The service used to publish and subscribe to <see cref="V1ResourceEvent"/>s</param>
    protected ResourceQueryHandlerBase(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus eventBus) 
        : base(loggerFactory)
    {
        this.Resources = resources;
        this.VersionControl = versionControl;
        this.EventBus= eventBus;
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
    /// Gets the service used to publish and subscribe to <see cref="V1ResourceEvent"/>s
    /// </summary>
    protected IResourceEventBus EventBus { get; }

}
