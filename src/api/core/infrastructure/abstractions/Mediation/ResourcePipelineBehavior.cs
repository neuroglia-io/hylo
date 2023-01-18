using Hylo.Api.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hylo.Api.Core.Infrastructure.Mediation;

/// <summary>
/// Represents the base class for all <see cref="ResourceCommand"/> <see cref="IPipelineBehavior{TRequest, TResponse}"/>s
/// </summary>
public abstract class ResourcePipelineBehaviorBase
    : IPipelineBehavior<ResourceCommand, IResponse<object>>
{

    /// <summary>
    /// Initializes a new <see cref="ResourcePipelineBehaviorBase"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resources">The service used to manage <see cref="V1Resource"/>s</param>
    public ResourcePipelineBehaviorBase(ILoggerFactory loggerFactory, IResourceRepository resources)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Resources = resources;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to manage <see cref="V1Resource"/>s
    /// </summary>
    protected IResourceRepository Resources { get; }

    /// <inheritdoc/>
    public abstract Task<IResponse<object>> Handle(ResourceCommand command, RequestHandlerDelegate<IResponse<object>> next, CancellationToken cancellationToken);

}
