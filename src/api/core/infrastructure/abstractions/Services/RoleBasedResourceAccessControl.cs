using IdentityModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Data;
using System.Reactive.Linq;
using System.Security.Claims;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of a <see cref="V1Role"/>/<see cref="V1ClusterRole"/>-based <see cref="IResourceAccessControl"/> interface
/// </summary>
public class RoleBasedResourceAccessControl
    : BackgroundService, IResourceAccessControl
{

    /// <summary>
    /// Initializes a new <see cref="RoleBasedResourceAccessControl"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resourceEventBus">The service used to publish and subscribe to <see cref="V1ResourceEvent"/></param>
    public RoleBasedResourceAccessControl(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IResourceEventBus resourceEventBus)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.ResourceEventBus = resourceEventBus;
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
    /// Gets the <see cref="RoleBasedResourceAccessControl"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that represents the <see cref="RoleBasedResourceAccessControl"/>'s subscription to role-related <see cref="V1ResourceEvent"/>s
    /// </summary> 
    protected IDisposable RoleEventSubscription { get; private set; } = null!;

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> used to map <see cref="V1Role"/>s in memory
    /// </summary>
    protected ConcurrentDictionary<string, RoleDescriptor> RoleMap { get; } = new();

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        await this.ReconcileAsync();
        this.RoleEventSubscription = this.ResourceEventBus
            .Select(e => e.ForResourceOfType<V1Resource<V1RoleSpec>>())
            .Where(e => 
                (e?.Resource.ApiVersion == ApiVersion.Build(V1ClusterRole.HyloGroup, V1ClusterRole.HyloApiVersion) && e?.Resource.Kind == V1ClusterRole.HyloKind) 
                || (e?.Resource.ApiVersion == ApiVersion.Build(V1Role.HyloGroup, V1Role.HyloApiVersion) && e?.Resource.Kind == V1Role.HyloKind))
            .Subscribe(OnRoleResourceEvent!);
    }

    /// <summary>
    /// Performs a reconciliation loop to ensure the state's validity
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ReconcileAsync()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IResourceRepository>();
        await foreach (var role in repository.ListResourcesAsync<V1ClusterRole>(V1ClusterRole.HyloGroup, V1ClusterRole.HyloApiVersion, V1ClusterRole.HyloPluralName, cancellationToken: this.CancellationTokenSource.Token))
        {
            this.RoleMap.TryAdd(role.Metadata.GetNamespacedName(), new(V1ResourceScope.Namespaced, role.Metadata.Namespace, role.Spec));
        }
        await foreach(var role in repository.ListResourcesAsync<V1Role>(V1Role.HyloGroup, V1Role.HyloApiVersion, V1Role.HyloPluralName, V1Namespace.All, cancellationToken: this.CancellationTokenSource.Token))
        {
            this.RoleMap.TryAdd(role.Metadata.GetNamespacedName(), new(V1ResourceScope.Namespaced, role.Metadata.Namespace, role.Spec));
        }
    }

    /// <inheritdoc/>
    public virtual Task<bool> AuthorizeResourceAccessAsync(ClaimsPrincipal user, string verb, string group, string version, string plural, string? name = null, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(verb)) throw new ArgumentNullException(nameof(verb));
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return Task.Run(() => user.FindAll(JwtClaimTypes.Role)
            .Where(c => this.RoleMap.ContainsKey(c.Value))
            .Select(c => this.RoleMap[c.Value])
            .Where(r => r.Scope == V1ResourceScope.Cluster || (r.Scope == V1ResourceScope.Namespaced && r.Namespace == @namespace))
            .Any(r => r.Authorizes(verb, group, plural, name)), cancellationToken);
    }

    /// <summary>
    /// Handles <see cref="V1ResourceEvent"/>s that affect <see cref="V1ClusterRole"/>s and <see cref="V1Role"/>s
    /// </summary>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual void OnRoleResourceEvent(V1ResourceEvent<V1Resource<V1RoleSpec>> e)
    {
        var roleScope = e.Resource.Kind switch
        {
            V1ClusterRole.HyloKind => V1ResourceScope.Cluster,
            V1Role.HyloKind => V1ResourceScope.Namespaced,
            _ => null
        };
        if (string.IsNullOrWhiteSpace(roleScope)) return;
        var namespacedName = e.Resource.Metadata.GetNamespacedName();
        var role = new RoleDescriptor(roleScope, e.Resource.Metadata.Namespace, e.Resource.Spec);
        switch (e.Type)
        {
            case V1ResourceEventType.Create:
            case V1ResourceEventType.Update:
                this.RoleMap.AddOrUpdate(namespacedName, role, (key, existing) => role);
                break;
            case V1ResourceEventType.Delete:
                this.RoleMap.TryRemove(namespacedName, out _);
                break;
        }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        this.CancellationTokenSource?.Dispose();
        this.RoleEventSubscription?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Represents an object to describe a role in memory
    /// </summary>
    protected class RoleDescriptor
    {

        /// <summary>
        /// Initializes a new <see cref="RoleDescriptor"/>
        /// </summary>
        /// <param name="scope">The scope of the described role<para></para>See <see cref="V1ResourceScope"/></param>
        /// <param name="namespace">The namespace the role applies to, if <see cref="Scope"/> is <see cref="V1ResourceScope.Namespaced"/></param>
        /// <param name="spec">The described role's configuration</param>
        public RoleDescriptor(string scope, string? @namespace, V1RoleSpec spec)
        {
            this.Scope = scope;
            this.Namespace = @namespace;
            this.Spec = spec;
        }

        /// <summary>
        /// Gets the scope of the described role<para></para>See <see cref="V1ResourceScope"/>
        /// </summary>
        public virtual string Scope { get; }

        /// <summary>
        /// Gets the namespace the role applies to, if <see cref="Scope"/> is <see cref="V1ResourceScope.Namespaced"/>
        /// </summary>
        public virtual string? Namespace { get; }

        /// <summary>
        /// Gets the described role's configuration
        /// </summary>
        public virtual V1RoleSpec Spec { get; set; }

        /// <summary>
        /// Determines whether or not the described role authorizes the specified operation
        /// </summary>
        /// <param name="verb">The verb that describes the operation to perform</param>
        /// <param name="group">The API group of the resource to operate on</param>
        /// <param name="name">The name of the resource to operate on</param>
        /// <returns>A boolean indicating whether or not the described role authorizes the specified operation</returns>
        public virtual bool Authorizes(string verb, string group, string plural, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(verb)) throw new ArgumentNullException(nameof(verb));
            if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
            if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
            return this.Spec.Rules?.Any(r => r.Authorizes(verb, group, plural, name)) == true;
        }

    }

}