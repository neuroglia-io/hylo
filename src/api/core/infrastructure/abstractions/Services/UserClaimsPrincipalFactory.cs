using IdentityModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IUserClaimsPrincipalFactory"/> interface
/// </summary>
public class UserClaimsPrincipalFactory
    : BackgroundService, IUserClaimsPrincipalFactory
{

    /// <summary>
    /// Initializes a new <see cref="UserClaimsPrincipalFactory"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resourceEventBus">The service used to publish and subscribe to <see cref="V1ResourceEvent"/></param>
    /// <param name="userRoleManager">The service used to manage roles</param>
    public UserClaimsPrincipalFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IResourceEventBus resourceEventBus, IUserRoleManager userRoleManager)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.ResourceEventBus = resourceEventBus;
        this.UserRoleManager = userRoleManager;
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
    /// Gets the service used to manage roles
    /// </summary>
    protected IUserRoleManager UserRoleManager { get; }

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> used to map in memory subjects to their Hylo claims
    /// </summary>
    protected ConcurrentDictionary<string, IEnumerable<Claim>> SubjectClaimsMap { get; } = new();

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> used to map in memory subjects to the role bindings they are associated to
    /// </summary>
    protected ConcurrentDictionary<string, List<string>> RoleBindingSubjectMap { get; } = new();

    /// <summary>
    /// Gets the <see cref="RoleBasedResourceAccessControl"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that represents the <see cref="UserClaimsPrincipalFactory"/>'s subscription to role binding-related <see cref="V1ResourceEvent"/>s
    /// </summary>
    protected IDisposable RoleBindingEventSubscription { get; private set; } = null!;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        await this.ReconcileAsync();
        this.RoleBindingEventSubscription = this.ResourceEventBus.Select(e => e.ForResourceOfType<V1Resource<V1RoleBindingSpec>>())
            .Where(e =>
                (e?.Resource.ApiVersion == ApiVersion.Build(V1ClusterRoleBinding.HyloGroup, V1ClusterRoleBinding.HyloApiVersion) && e?.Resource.Kind == V1ClusterRoleBinding.HyloKind)
                || (e?.Resource.ApiVersion == ApiVersion.Build(V1RoleBinding.HyloGroup, V1RoleBinding.HyloApiVersion) && e?.Resource.Kind == V1RoleBinding.HyloKind))
            .SubscribeAsync(OnRoleBindingResourceEventAsync, this.CancellationTokenSource.Token);
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
            var subjects = roleBinding.Spec.Subjects.Select(s => s.Name).ToList();
            this.RoleBindingSubjectMap.AddOrUpdate(roleBinding.Metadata.GetNamespacedName(), subjects, (key, existing) => subjects);
        }
        await foreach (var roleBinding in repository.ListResourcesAsync<V1RoleBinding>(V1RoleBinding.HyloGroup, V1RoleBinding.HyloApiVersion, V1RoleBinding.HyloPluralName, V1Namespace.All, cancellationToken: this.CancellationTokenSource.Token))
        {
            var subjects = roleBinding.Spec.Subjects.Select(s => s.Name).ToList();
            this.RoleBindingSubjectMap.AddOrUpdate(roleBinding.Metadata.GetNamespacedName(), subjects, (key, existing) => subjects);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ClaimsPrincipal?> CreateAsync(string subject, string authenticationType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
        if (string.IsNullOrWhiteSpace(authenticationType)) throw new ArgumentNullException(nameof(authenticationType));
        if (!this.SubjectClaimsMap.TryGetValue(subject, out var claims)) return null;
        return await Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType, JwtClaimTypes.Name, JwtClaimTypes.Role)));
    }

    /// <summary>
    /// Generates <see cref="Claim"/>s for the specified subject
    /// </summary>
    /// <param name="subject">The subject to generate <see cref="Claim"/>s for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to asynchronously enumerate the generated <see cref="Claim"/>s</returns>
    protected virtual async IAsyncEnumerable<Claim> GenerateClaimsAsync(string subject, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
        yield return new(JwtClaimTypes.Name, subject);
        await foreach(var role in this.UserRoleManager.GetRolesAsync(subject, cancellationToken))
        {
            yield return new(JwtClaimTypes.Role, role);
        }
    }

    /// <summary>
    /// Handles <see cref="V1ResourceEvent"/>s that affect <see cref="V1ClusterRoleBinding"/>s and <see cref="V1RoleBinding"/>s
    /// </summary>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task OnRoleBindingResourceEventAsync(V1ResourceEvent<V1Resource<V1RoleBindingSpec>> e)
    {
        try
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var bindingName = e.Resource.Metadata.GetNamespacedName();
            var roleName = e.Resource.Metadata.GetNamespacedName();
            switch (e.Type)
            {
                case V1ResourceEventType.Create:
                    var subjects = e.Resource.Spec.Subjects.Select(s => s.Name).ToList();
                    this.RoleBindingSubjectMap.AddOrUpdate(bindingName, subjects, (key, existing) => subjects);
                    foreach (var subject in e.Resource.Spec.Subjects)
                    {
                        this.SubjectClaimsMap[subject.Name] = await this.GenerateClaimsAsync(subject.Name, this.CancellationTokenSource.Token).ToListAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    break;
                case V1ResourceEventType.Update:
                    var subjectsPerRoleBinding = e.Resource.Spec.Subjects.Select(s => s.Name).ToList();
                    if (this.RoleBindingSubjectMap.TryGetValue(bindingName, out var results)) subjectsPerRoleBinding.AddRange(results);
                    foreach (var subject in subjectsPerRoleBinding.Distinct().ToList())
                    {
                        this.SubjectClaimsMap[subject] = await this.GenerateClaimsAsync(subject, this.CancellationTokenSource.Token).ToListAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    this.RoleBindingSubjectMap[bindingName] = e.Resource.Spec.Subjects.Select(s => s.Name).ToList();
                    break;
                case V1ResourceEventType.Delete:
                    subjectsPerRoleBinding = e.Resource.Spec.Subjects.Select(s => s.Name).ToList();
                    if (this.RoleBindingSubjectMap.TryGetValue(bindingName, out results)) subjectsPerRoleBinding.AddRange(results);
                    foreach (var subject in subjectsPerRoleBinding.Distinct().ToList())
                    {
                        this.SubjectClaimsMap[subject] = await this.GenerateClaimsAsync(subject, this.CancellationTokenSource.Token).ToListAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    this.RoleBindingSubjectMap.Remove(bindingName, out _);
                    break;
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError("An error occured while handling a role binding related event: {ex}", ex);
        }
    }

}
