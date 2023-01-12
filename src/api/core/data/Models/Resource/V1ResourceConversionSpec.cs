namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure the conversion of related <see cref="V1Resource"/>s to different <see cref="V1ResourceDefinition"/> versions
/// </summary>
[DataContract]
public class V1ResourceConversionSpec
{

    /// <summary>
    /// Gets the conversion strategy to use<para></para>
    /// <see cref="V1ResourceConversionStrategies">View default values</see>
    /// </summary>
    [DataMember(Name = "strategy", Order = 1), JsonPropertyName("strategy")]
    public virtual string? Strategy { get; set; }

    /// <summary>
    /// Gets the object used to configure the webhook to invoke when <see cref="Strategy"/> has been set to <see cref="V1ResourceConversionStrategies.Webhook"/>
    /// </summary>
    [DataMember(Name = "webhook", Order = 2), JsonPropertyName("webhook")]
    public virtual V1WebhookResourceConversion? Webhook { get; set; }

}
