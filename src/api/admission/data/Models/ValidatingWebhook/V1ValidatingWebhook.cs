namespace Hylo.Api.Admission.Data.Models;

/// <summary>
/// Represents a webhook used to validate <see cref="V1Resource"/>s
/// </summary>
[Resource(HyloGroup, HyloApiVersion, HyloKind, HyloPluralName), DataContract]
public class V1ValidatingWebhook
    : V1Resource<V1ValidatingWebhookSpec>
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
    public const string HyloKind = "ValidatingWebhook";
    /// <summary>
    /// Gets the resource plural name
    /// </summary>
    public const string HyloPluralName = "validating-webhook";

    /// <summary>
    /// Initializes a new <see cref="V1ValidatingWebhook"/>
    /// </summary>
    public V1ValidatingWebhook() : base(HyloGroup, HyloApiVersion, HyloKind) { }

    /// <summary>
    /// Initializes a new <see cref="V1ValidatingWebhook"/>
    /// </summary>
    /// <param name="spec">The <see cref="V1ValidatingWebhook"/>'s <see cref="V1ValidatingWebhookSpec"/></param>
    public V1ValidatingWebhook(V1ResourceMetadata metadata, V1ValidatingWebhookSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
