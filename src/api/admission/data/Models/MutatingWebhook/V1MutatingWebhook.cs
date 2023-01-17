namespace Hylo.Api.Admission.Data.Models;

/// <summary>
/// Represents a webhook used to validate <see cref="V1Resource"/>s
/// </summary>
[Resource(HyloGroup, HyloApiVersion, HyloKind, HyloPluralName), DataContract]
public class V1MutatingWebhook
    : V1Resource<V1MutatingWebhookSpec>
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public const string HyloGroup = V1AdmissionApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public const string HyloApiVersion = V1AdmissionApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public const string HyloKind = "MutatingWebhook";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "mutating-webhook";

    /// <summary>
    /// Initializes a new <see cref="V1MutatingWebhook"/>
    /// </summary>
    public V1MutatingWebhook() : base(HyloGroup, HyloApiVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1MutatingWebhook"/>
    /// </summary>
    /// <param name="spec">The <see cref="V1MutatingWebhook"/>'s <see cref="V1MutatingWebhookSpec"/></param>
    public V1MutatingWebhook(V1ResourceMetadata metadata, V1MutatingWebhookSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
