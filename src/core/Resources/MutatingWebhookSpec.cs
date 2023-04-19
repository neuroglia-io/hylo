namespace Hylo.Resources;

/// <summary>
/// Represents the specification of a <see cref="MutatingWebhook"/>'s 
/// </summary>
[DataContract]
public class MutatingWebhookSpec
{

    /// <summary>
    /// Initializes a new <see cref="MutatingWebhookSpec"/>
    /// </summary>
    public MutatingWebhookSpec() { }

    /// <summary>
    /// Initializes a new <see cref="MutatingWebhookSpec"/>
    /// </summary>
    /// <param name="client">The <see cref="WebhookClientConfiguration"/> used to configure the webhook to call</param>
    /// <param name="resources">An <see cref="IEnumerable{T}"/> containing the filters used to configure when to call the <see cref="MutatingWebhook"/></param>
    /// <param name="priority">The <see cref="MutatingWebhook"/>'s priority</param>
    public MutatingWebhookSpec(WebhookClientConfiguration client, IEnumerable<RuleWithOperation>? resources = null, int? priority = null)
    {
        this.Client = client ?? throw new ArgumentNullException(nameof(client));
        this.Resources = resources?.ToList();
        this.Priority = priority;
    }

    /// <summary>
    /// Gets the <see cref="WebhookClientConfiguration"/> used to configure the webhook to call
    /// </summary>
    [Required]
    [DataMember(Name = "client", Order = 1), JsonPropertyOrder(1), JsonPropertyName("client"), YamlMember(Order = 2, Alias = "client")]
    public virtual WebhookClientConfiguration Client { get; set; } = null!;

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the filters used to configure when to call the <see cref="MutatingWebhook"/>
    /// </summary>
    [DataMember(Name = "resources", Order = 2), JsonPropertyOrder(2), JsonPropertyName("resources"), YamlMember(Order = 2, Alias = "resources")]
    public virtual List<RuleWithOperation>? Resources { get; set; }

    /// <summary>
    /// Gets the <see cref="MutatingWebhook"/>'s priority
    /// </summary>
    [DataMember(Name = "priority", Order = 3), JsonPropertyOrder(3), JsonPropertyName("priority"), YamlMember(Order = 3, Alias = "priority")]
    public virtual int? Priority { get; set; }

}
