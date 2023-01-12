namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure a webhook-based <see cref="V1ResourceConversionSpec"/>
/// </summary>
[DataContract]
public class V1WebhookResourceConversion
{

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the version supported by the webhook conversion
    /// </summary>
    [DataMember(Name = "supportedVersions", Order = 1), JsonPropertyName("supportedVersions"), Required]
    public virtual List<string>? SupportedVersions { get; set; }

    /// <summary>
    /// Gets the object used to configure the webhook to call
    /// </summary>
    [DataMember(Name = "client", Order = 2), JsonPropertyName("client"), Required]
    public virtual V1WebhookClientConfiguration Client { get; set; } = null!;

}
