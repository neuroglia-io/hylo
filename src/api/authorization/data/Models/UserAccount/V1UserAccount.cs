namespace Hylo.Api.Authorization.Data.Models;

/// <summary>
/// Represents a user account
/// </summary>
[DataContract]
public class V1UserAccount
    : V1Resource<V1UserAccountSpec>
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public const string HyloGroup = V1AuthorizationApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public const string HyloApiVersion = V1AuthorizationApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public const string HyloKind = "UserAccount";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "user-account";

    /// <summary>
    /// Initializes a new <see cref="V1UserAccount"/>
    /// </summary>
    public V1UserAccount() : base(HyloGroup, HyloApiVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1UserAccount"/>
    /// </summary>
    /// <param name="spec">The <see cref="V1UserAccount"/>'s <see cref="V1UserAccountSpec"/></param>
    public V1UserAccount(V1ResourceMetadata metadata, V1UserAccountSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
