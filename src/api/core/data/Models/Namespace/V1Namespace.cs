namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a <see cref="V1Resource"/> namespace
/// </summary>
[Resource(HyloGroup, HyloApiVersion, HyloKind, HyloPluralName), DataContract]
public class V1Namespace
    : V1Resource
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
    public const string HyloKind = "Namespace";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "namespaces";

    /// <summary>
    /// Gets the 'default' namespace
    /// </summary>
    public const string Default = "default";
    /// <summary>
    /// Gets the 'all' reserved namespace, used to list resources accross namespaces
    /// </summary>
    public const string All = "all";

    /// <summary>
    /// Initializes a new <see cref="V1Namespace"/>
    /// </summary>
    public V1Namespace() : base(HyloGroup, HyloApiVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1Namespace"/>
    /// </summary>
    /// <param name="metadata">The <see cref="V1Namespace"/>'s metadata</param>
    public V1Namespace(V1ResourceMetadata metadata)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

}
