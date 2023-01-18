namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the definition of a resource
/// </summary>
[Resource(HyloGroup, HyloVersion, HyloKind, HyloPluralName), DataContract]
public class V1ResourceDefinition
    : V1Resource<V1ResourceDefinitionSpec>
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public const string HyloGroup = V1CoreApiDefaults.Resources.Group;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public const string HyloVersion = V1CoreApiDefaults.Resources.Version;
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public const string HyloKind = "ResourceDefinition";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "resource-definitions";

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinition"/>
    /// </summary>
    public V1ResourceDefinition() : base(HyloGroup, HyloVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinition"/>
    /// </summary>
    /// <param name="spec">The <see cref="V1ResourceDefinition"/>'s <see cref="V1ResourceDefinitionSpec"/></param>
    public V1ResourceDefinition(V1ResourceMetadata metadata, V1ResourceDefinitionSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
