namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a resource used to configure bindings of subjects to a <see cref="V1ClusterRole"/>
/// </summary>
[DataContract]
public class V1ClusterRoleBinding
    : V1Resource<V1RoleBindingSpec>
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public const string HyloGroup = V1CoreApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public const string HyloApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public const string HyloKind = "ClusterRoleBinding";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "cluster-role-bindings";

    /// <summary>
    /// Initializes a new <see cref="V1ClusterRoleBinding"/>
    /// </summary>
    public V1ClusterRoleBinding() : base(HyloGroup, HyloApiVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1ClusterRoleBinding"/>
    /// </summary>
    /// <param name="spec">The <see cref="V1ClusterRoleBinding"/>'s <see cref="V1RoleBindingSpec"/></param>
    public V1ClusterRoleBinding(V1ResourceMetadata metadata, V1RoleBindingSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
