namespace Hylo.Api.Authorization.Data.Models;

/// <summary>
/// Represents a role that applies cluster-wide
/// </summary>
[Resource(HyloGroup, HyloApiVersion, HyloKind, HyloPluralName), DataContract]
public class V1ClusterRole
    : V1Resource<V1RoleSpec>
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public const string HyloGroup = V1AuthorizationApiDefaults.Resources.Group;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public const string HyloApiVersion = V1AuthorizationApiDefaults.Resources.Version;
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public const string HyloKind = "ClusterRole";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "cluster-roles";

    /// <summary>
    /// Initializes a new <see cref="V1ClusterRole"/>
    /// </summary>
    public V1ClusterRole() : base(HyloGroup, HyloApiVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1ClusterRole"/>
    /// </summary>
    /// <param name="spec">The <see cref="V1ClusterRole"/>'s <see cref="V1RoleSpec"/></param>
    public V1ClusterRole(V1ResourceMetadata metadata, V1RoleSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
