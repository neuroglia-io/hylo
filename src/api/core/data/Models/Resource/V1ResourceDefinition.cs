namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the definition of a resource
/// </summary>
public class V1ResourceDefinition
    : V1Resource<V1ResourceDefinitionSpec>
{

    /// <summary>
    /// Gets the <see cref="V1ResourceDefinition"/> resource API version
    /// </summary>
    public const string ResourceApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the <see cref="V1ResourceDefinition"/> resource kind
    /// </summary>
    public const string ResourceKind = "ResourceDefinition";

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinition"/>
    /// </summary>
    public V1ResourceDefinition() { }

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
