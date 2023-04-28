using Hylo.Resources;

namespace Hylo;

/// <summary>
/// Represents the object used to configure a webhook-based <see cref="ResourceConversion"/>
/// </summary>
[DataContract]
public record WebhookResourceConversion
{

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the version supported by the webhook conversion
    /// </summary>
    [Required]
    [DataMember(Name = "supportedVersions", Order = 1), JsonPropertyOrder(1), JsonPropertyName("supportedVersions"), YamlMember(Order = 1, Alias = "supportedVersions")]
    public virtual EquatableList<string>? SupportedVersions { get; set; }

    /// <summary>
    /// Gets the object used to configure the webhook to call
    /// </summary>
    [Required]
    [DataMember(Name = "client", Order = 2), JsonPropertyOrder(2), JsonPropertyName("client"), YamlMember(Order = 2, Alias = "client")]
    public virtual WebhookClientConfiguration Client { get; set; } = null!;

}
