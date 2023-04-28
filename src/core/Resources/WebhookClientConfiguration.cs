namespace Hylo.Resources;

/// <summary>
/// Represents the object used to configure a webhook client
/// </summary>
[DataContract]
public record WebhookClientConfiguration
{

    /// <summary>
    /// Initializes a new <see cref="WebhookClientConfiguration"/>
    /// </summary>
    public WebhookClientConfiguration() { }

    /// <summary>
    /// Initializes a new <see cref="WebhookClientConfiguration"/>
    /// </summary>
    /// <param name="uri">The <see cref="System.Uri"/> of the remote service to call</param>
    public WebhookClientConfiguration(Uri uri)
    {
        this.Uri = uri;
    }

    /// <summary>
    /// Gets the <see cref="System.Uri"/> of the remote service to call
    /// </summary>
    [Required]
    [DataMember(Name = "uri", Order = 1), JsonPropertyOrder(1), JsonPropertyName("uri"), YamlMember(Order = 1, Alias = "uri")]
    public virtual Uri Uri { get; set; } = null!;

}