namespace Hylo;

/// <summary>
/// Represents the object used to configure the conversion of related <see cref="IResource"/>s to different <see cref="IResourceDefinition"/> versions
/// </summary>
[DataContract]
public class ResourceConversion
{

    /// <summary>
    /// Initializes a new <see cref="ResourceConversion"/>
    /// </summary>
    public ResourceConversion() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceConversion"/>
    /// </summary>
    /// <param name="webhook">The object used to configure the webhook to invoke</param>
    public ResourceConversion(V1WebhookResourceConversion webhook)
    {
        this.Strategy = ConversionStrategy.Webhook;
        this.Webhook = webhook ?? throw new ArgumentNullException(nameof(webhook));
    }

    /// <summary>
    /// Gets the conversion strategy to use
    /// </summary>
    [DataMember(Name = "strategy", Order = 1), JsonPropertyOrder(1), JsonPropertyName("strategy"), YamlMember(Order = 1, Alias = "strategy")]
    public virtual string? Strategy { get; set; }

    /// <summary>
    /// Gets the object used to configure the webhook to invoke when strategy has been set to 'webhook'
    /// </summary>
    [DataMember(Name = "webhook", Order = 2), JsonPropertyOrder(2), JsonPropertyName("webhook"), YamlMember(Order = 2, Alias = "strategy")]
    public virtual V1WebhookResourceConversion? Webhook { get; set; }

}
