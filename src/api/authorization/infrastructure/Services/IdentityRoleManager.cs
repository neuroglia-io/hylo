using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Security;

namespace Hylo.Api.Authorization.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IIdentityRoleManager"/>
/// </summary>
public class IdentityRoleManager
    : BackgroundService, IIdentityRoleManager
{

    /// <summary>
    /// Initializes a new <see cref="IdentityRoleManager"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resourceEventBus">The service used to publish and subscribe to <see cref="V1ResourceEvent"/></param>
    public IdentityRoleManager(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IResourceEventBus resourceEventBus)
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
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> used to map roles to subjects in memory
    /// </summary>
    protected ConcurrentDictionary<string, Dictionary<string, string>> SubjectRolesMap { get; } = new();

    /// <summary>
    /// Gets the <see cref="RoleBasedResourceAccessControl"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that represents the subscription to <see cref="V1UserAccount"/>-related <see cref="V1ResourceEvent"/>s
    /// </summary>
    protected IDisposable UserAccountEventSubscription { get; private set; } = null!;

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that represents the subscription to <see cref="V1UserAccount"/>-related <see cref="V1ResourceEvent"/>s
    /// </summary>
    protected IDisposable RoleBindingEventSubscription { get; private set; } = null!;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        await this.ReconcileAsync();
        this.UserAccountEventSubscription = this.ResourceEventBus
            .Select(e => e.ForResourceOfType<V1UserAccount>())
            .Where(e => e != null && e.Resource.ApiVersion == ApiVersion.Build(V1UserAccount.HyloGroup, V1UserAccount.HyloApiVersion) && e?.Resource.Kind == V1UserAccount.HyloKind)
            .Subscribe(this.OnUserAccountResourceEvent!);
    }

    /// <summary>
    /// Performs a reconciliation loop to ensure the state's validity
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ReconcileAsync()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IResourceRepository>();
        await foreach (var roleBinding in repository.ListResourcesAsync<V1ClusterRoleBinding>(V1ClusterRoleBinding.HyloGroup, V1ClusterRoleBinding.HyloApiVersion, V1ClusterRoleBinding.HyloPluralName, cancellationToken: this.CancellationTokenSource.Token))
        {
            var bindingName = roleBinding.Metadata.GetNamespacedName();
            var roleName = roleBinding.Spec.Role.GetNamespacedName();
            foreach (var subject in roleBinding.Spec.Subjects)
            {
                this.SubjectRolesMap.AddOrUpdate(subject.Name, new Dictionary<string, string>() { { bindingName, roleName } }, (key, existing) =>
                {
                    existing.Add(bindingName, roleName);
                    return existing;
                });
            }
        }
        await foreach (var roleBinding in repository.ListResourcesAsync<V1RoleBinding>(V1RoleBinding.HyloGroup, V1RoleBinding.HyloApiVersion, V1RoleBinding.HyloPluralName, V1Namespace.All, cancellationToken: this.CancellationTokenSource.Token))
        {
            var bindingName = roleBinding.Metadata.GetNamespacedName();
            var roleName = roleBinding.Spec.Role.GetNamespacedName();
            foreach (var subject in roleBinding.Spec.Subjects)
            {
                this.SubjectRolesMap.AddOrUpdate(subject.Name, new Dictionary<string, string>() { { bindingName, roleName } }, (key, existing) =>
                {
                    existing.Add(bindingName, roleName);
                    return existing;
                });
            }
        }
    }

    /// <summary>
    /// Handles <see cref="V1ResourceEvent"/>s that affect <see cref="V1UserAccount"/>s
    /// </summary>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual void OnUserAccountResourceEvent(V1ResourceEvent<V1UserAccount> e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        if (e.Type != V1ResourceEventType.Deleted) return;
        this.SubjectRolesMap.Remove(e.Resource.Metadata.Name, out _);
    }

    /// <summary>
    /// Handles <see cref="V1ResourceEvent"/>s that affect <see cref="V1ClusterRoleBinding"/>s and <see cref="V1RoleBinding"/>s
    /// </summary>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual void OnRoleBindingResourceEvent(V1ResourceEvent<V1Resource<V1RoleBindingSpec>> e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        var bindingName = e.Resource.Metadata.GetNamespacedName();
        var roleName = e.Resource.Metadata.GetNamespacedName();
        switch (e.Type)
        {
            case V1ResourceEventType.Created:
                foreach (var subject in e.Resource.Spec.Subjects)
                {
                    this.SubjectRolesMap.AddOrUpdate(subject.Name, new Dictionary<string, string>() { { bindingName, roleName } }, (key, existing) =>
                    {
                        existing.Add(bindingName, roleName);
                        return existing;
                    });
                }
                break;
            case V1ResourceEventType.Updated:
                foreach (var rolesPerSubject in this.SubjectRolesMap.ToList())
                {
                    rolesPerSubject.Value.Remove(bindingName);
                }
                foreach (var subject in e.Resource.Spec.Subjects)
                {
                    this.SubjectRolesMap.AddOrUpdate(subject.Name, new Dictionary<string, string>() { { bindingName, roleName } }, (key, existing) =>
                    {
                        existing.Add(bindingName, roleName);
                        return existing;
                    });
                }
                break;
            case V1ResourceEventType.Deleted:
                foreach (var rolesPerSubject in this.SubjectRolesMap.ToList())
                {
                    rolesPerSubject.Value.Remove(bindingName);
                }
                break;
        }
    }

    public virtual IAsyncEnumerable<string> GetRolesAsync(string subject, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
        if (!this.SubjectRolesMap.TryGetValue(subject, out var roles)) throw new SecurityException("Unknown user");
        return roles.Values.Distinct().ToAsyncEnumerable();
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        this.CancellationTokenSource?.Dispose();
        this.UserAccountEventSubscription?.Dispose();
        this.RoleBindingEventSubscription?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }

}
