namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure a webhook client
/// </summary>
[DataContract]
public class V1WebhookClientConfiguration
{

    /// <summary>
    /// Initializes a new <see cref="V1WebhookClientConfiguration"/>
    /// </summary>
    public V1WebhookClientConfiguration() { }

    /// <summary>
    /// Initializes a new <see cref="V1WebhookClientConfiguration"/>
    /// </summary>
    /// <param name="uri">The <see cref="System.Uri"/> of the remote service to call</param>
    public V1WebhookClientConfiguration(Uri uri)
    {
        this.Uri = uri;
    }

    /// <summary>
    /// Gets the <see cref="System.Uri"/> of the remote service to call
    /// </summary>
    [DataMember(Name = "uri", Order = 1), JsonPropertyName("uri"), Required]
    public virtual Uri Uri { get; set; } = null!;

}
