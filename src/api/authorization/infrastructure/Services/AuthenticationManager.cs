using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Security.Cryptography;

namespace Hylo.Api.Authorization.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IAuthenticationManager"/>
/// </summary>
public class AuthenticationManager
    : BackgroundService, IAuthenticationManager
{

    /// <summary>
    /// Initializes a new <see cref="AuthenticationManager"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resourceEventBus">The service used to publish and subscribe to <see cref="V1ResourceEvent"/></param>
    public AuthenticationManager(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IResourceEventBus resourceEventBus, IIdentityClaimsPrincipalFactory claimsPrincipalFactory)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.ResourceEventBus = resourceEventBus;
        this.ClaimsPrincipalFactory = claimsPrincipalFactory;
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
    /// Gets the service used to create <see cref="ClaimsPrincipal"/>s
    /// </summary>
    protected IIdentityClaimsPrincipalFactory ClaimsPrincipalFactory { get; }

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> used to map user accounts in memory
    /// </summary>
    protected ConcurrentDictionary<string, V1UserAccount> UserAccountMap { get; } = new();

    /// <summary>
    /// Gets the <see cref="RoleBasedResourceAccessControl"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that represents the subscription to <see cref="V1UserAccount"/>-related <see cref="V1ResourceEvent"/>s
    /// </summary>
    protected IDisposable UserAccountEventSubscription { get; private set; } = null!;

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
        await foreach(var userAccount in repository.ListResourcesAsync<V1UserAccount>(V1UserAccount.HyloGroup, V1UserAccount.HyloApiVersion, V1UserAccount.HyloPluralName, cancellationToken: this.CancellationTokenSource.Token))
        {
            this.UserAccountMap.AddOrUpdate(userAccount.Metadata.Name, userAccount, (key, existing) => userAccount);
        }
    }
    
    /// <inheritdoc/>
    public virtual async Task<AuthenticateResult> AuthenticateAsync(string authenticationScheme, object authenticateProperties, CancellationToken cancellationToken = default)
    {
        var subject = string.Empty;
        V1UserAccount? account = null;
        switch (authenticateProperties)
        {
            case BasicAuthenticationProperties basic:
                subject = basic.Username;
                if (!this.UserAccountMap.TryGetValue(subject, out account) || account == null) return AuthenticateResult.Fail("Unknown subject");
                if (HashHelper.Hash(HashAlgorithmName.SHA512, basic.Password, account.Spec.Authentication.Basic!.PasswordSalt) != account.Spec.Authentication.Basic!.PasswordHash) return AuthenticateResult.Fail("Invalid username and/or password");
                break;
            case CertificateAuthenticationProperties clientCertificate:
                subject = clientCertificate!.Certificate.Subject;
                if (!this.UserAccountMap.TryGetValue(subject, out account) || account == null) return AuthenticateResult.Fail("Unknown subject");
                if (account.Spec.Authentication.ClientCertificate == null) return AuthenticateResult.Fail("Invalid/unconfigured credentials type");
                if (clientCertificate.Certificate.Thumbprint != account.Spec.Authentication.ClientCertificate.Certificate.Thumbprint) return AuthenticateResult.Fail("Invalid certificate");
                break;
            default:
                return AuthenticateResult.Fail("Unsupported credentials type");
        }
        var claimsPrincipal = await this.ClaimsPrincipalFactory.CreateAsync(subject, authenticationScheme, cancellationToken);
        if (claimsPrincipal == null) return AuthenticateResult.Fail("Failed to resolve the subject's identity");
        return AuthenticateResult.Success(new(claimsPrincipal, authenticationScheme));
    }

    /// <summary>
    /// Handles <see cref="V1ResourceEvent"/>s that affect <see cref="V1UserAccount"/>s
    /// </summary>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual void OnUserAccountResourceEvent(V1ResourceEvent<V1UserAccount> e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        switch (e.Type)
        {
            case V1ResourceEventType.Created:
                this.UserAccountMap.AddOrUpdate(e.Resource.Metadata.Name, e.Resource, (key, existing) => e.Resource);
                break;
            case V1ResourceEventType.Updated:
                this.UserAccountMap.AddOrUpdate(e.Resource.Metadata.Name, e.Resource, (key, existing) => e.Resource);
                break;
            case V1ResourceEventType.Deleted:
                this.UserAccountMap.Remove(e.Resource.Metadata.Name, out _);
                break;
        }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        this.CancellationTokenSource?.Dispose();
        this.UserAccountEventSubscription?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }

}
