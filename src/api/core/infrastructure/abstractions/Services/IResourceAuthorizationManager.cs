using IdentityModel;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Security.Claims;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to authorize users to operate on resources
/// </summary>
public interface IResourceAccessControl
{

    /// <summary>
    /// Authorize a user to perform an operation on the specified resource
    /// </summary>
    /// <param name="user">The user to authorize</param>
    /// <param name="operation">The operation to perform</param>
    /// <param name="resource">The resource to operate on</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task AuthorizeResourceAccessAsync(ClaimsPrincipal user, string operation, object resource, CancellationToken cancellationToken = default);

}

/// <summary>
/// Represents the default implementation of a <see cref="V1Role"/>/<see cref="V1ClusterRole"/>-based <see cref="IResourceAccessControl"/> interface
/// </summary>
public class ResourceRoleBasedAccessControl
    : BackgroundService, IResourceAccessControl
{

    /// <summary>
    /// Initializes a new <see cref="ResourceRoleBasedAccessControl"/>
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="resourceEventBus"></param>
    /// <param name="cache"></param>
    public ResourceRoleBasedAccessControl(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IResourceEventBus resourceEventBus, IMemoryCache cache)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.ResourceEventBus = resourceEventBus;
        this.Cache = cache;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to publish and subscribe to <see cref="V1ResourceEvent"/>
    /// </summary>
    protected IResourceEventBus ResourceEventBus { get; }

    /// <summary>
    /// Gets the service used to manage the cache used to persist <see cref="V1Role"/>s in memory
    /// </summary>
    protected IMemoryCache Cache { get; }

    /// <summary>
    /// Gets the <see cref="ResourceRoleBasedAccessControl"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; }

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that represents the <see cref="ResourceRoleBasedAccessControl"/>'s subscription to the <see cref="ResourceEventBus"/>
    /// </summary>
    protected IDisposable Subscription { get; private set; }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        this.Subscription = this.ResourceEventBus.Where(e => e.Concerns(V1Role.ResourceApiVersion, V1Role.ResourceKind) || e.Concerns(V1ClusterRole.ResourceApiVersion, V1ClusterRole.ResourceKind)).Subscribe();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task AuthorizeResourceAccessAsync(ClaimsPrincipal user, string operation, object resource, CancellationToken cancellationToken = default)
    {
        if(user == null) throw new ArgumentNullException(nameof(user));
        if(string.IsNullOrWhiteSpace(operation)) throw new ArgumentNullException(nameof(operation));
        if(resource == null) throw new ArgumentNullException(nameof(resource));
        var roleNames = user.Claims.Where(c => c.Type == JwtClaimTypes.Role).Select(c => c.Value).ToList();

    }

    protected virtual async Task OnResourceEventAsync(V1ResourceEvent e)
    {
        switch (e.EventType)
        {
            case V1ResourceEventType.ResourceCreated:

                break;
            case V1ResourceEventType.ResourceUpdated:

                break;
            case V1ResourceEventType.ResourceDeleted:

                break;
        }
    }

}

public interface IResourceController
    : IDisposable, IAsyncDisposable
{

    Task ReconcileAsync(CancellationToken cancellationToken = default);

}

public interface IResourceController<TResource>
    : IDisposable, IAsyncDisposable
{

    Task ReconcileAsync(CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of a service used to publish and subscribe to <see cref="V1ResourceEvent"/>s
/// </summary>
public interface IResourceEventBus
    : IObservable<V1ResourceEvent>
{

    /// <summary>
    /// Publishes the specified <see cref="V1ResourceEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to publish</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task PublishAsync(V1ResourceEvent e, CancellationToken cancellationToken = default);

}