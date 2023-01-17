namespace Hylo.Api.Admission.Data.Models;

/// <summary>
/// Represents the specification of a <see cref="V1ValidatingWebhook"/>'s 
/// </summary>
[DataContract]
public class V1ValidatingWebhookSpec
{

    /// <summary>
    /// Initializes a new <see cref="V1ValidatingWebhookSpec"/>
    /// </summary>
    public V1ValidatingWebhookSpec() { }

    /// <summary>
    /// Initializes a new <see cref="V1ValidatingWebhookSpec"/>
    /// </summary>
    /// <param name="client">The <see cref="V1WebhookClientConfiguration"/> used to configure the webhook to call</param>
    /// <param name="resources">An <see cref="IEnumerable{T}"/> containing the filters used to configure when to call the <see cref="V1ValidatingWebhook"/></param>
    /// <param name="priority">The <see cref="V1ValidatingWebhook"/>'s priority</param>
    public V1ValidatingWebhookSpec(V1WebhookClientConfiguration client, IEnumerable<V1RuleWithOperation>? resources = null, long? priority = null)
    {
        this.Resources = resources?.ToList();
        this.Priority = priority;
    }

    /// <summary>
    /// Gets the <see cref="V1WebhookClientConfiguration"/> used to configure the webhook to call
    /// </summary>
    [DataMember(Name = "client", Order = 1), JsonPropertyName("client"), Required]
    public virtual V1WebhookClientConfiguration Client { get; set; } = null!;

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the filters used to configure when to call the <see cref="V1ValidatingWebhook"/>
    /// </summary>
    [DataMember(Name = "resources", Order = 2), JsonPropertyName("resources")]
    public virtual List<V1RuleWithOperation>? Resources { get; set; }

    /// <summary>
    /// Gets the <see cref="V1ValidatingWebhook"/>'s priority
    /// </summary>
    [DataMember(Name = "priority", Order = 3), JsonPropertyName("priority")]
    public virtual long? Priority { get; set; }

}
