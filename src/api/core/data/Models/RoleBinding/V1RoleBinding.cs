namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a resource used to configure bindings of subjects to a <see cref="V1Role"/>
/// </summary>
[DataContract]
public class V1RoleBinding
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
    public const string HyloKind = "RoleBinding";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "role-bindings";

    /// <summary>
    /// Initializes a new <see cref="V1RoleBinding"/>
    /// </summary>
    public V1RoleBinding() : base(HyloGroup, HyloApiVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1RoleBinding"/>
    /// </summary>
    /// <param name="spec">The <see cref="V1RoleBinding"/>'s <see cref="V1RoleBindingSpec"/></param>
    public V1RoleBinding(V1ResourceMetadata metadata, V1RoleBindingSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
