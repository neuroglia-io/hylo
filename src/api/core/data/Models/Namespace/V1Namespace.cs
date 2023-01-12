namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a <see cref="V1Resource"/> namespace
/// </summary>
public class V1Namespace
    : V1Resource
{

    /// <summary>
    /// Gets the <see cref="V1ResourceDefinition"/> resource API version
    /// </summary>
    public const string ResourceApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the <see cref="V1ResourceDefinition"/> resource kind
    /// </summary>
    public const string ResourceKind = "Namespace";

    /// <summary>
    /// Initializes a new <see cref="V1Namespace"/>
    /// </summary>
    public V1Namespace() : base(ResourceApiVersion, ResourceKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1Namespace"/>
    /// </summary>
    /// <param name="metadata">The <see cref="V1Namespace"/>'s metadata</param>
    public V1Namespace(V1ResourceMetadata metadata)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    /// <summary>
    /// Gets the 'default' namespace
    /// </summary>
    public const string Default = "default";

}
