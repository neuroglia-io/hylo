using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Represents a message exchanged between Hylo API server instances
/// </summary>
[DataContract]
public class ApiServerMessage
    : IExtensible
{

    /// <summary>
    /// Initializes a new <see cref="ApiServerMessage"/>
    /// </summary>
    public ApiServerMessage() { }

    /// <summary>
    /// Initializes a new <see cref="ApiServerMessage"/>
    /// </summary>
    /// <param name="type">The <see cref="ApiServerMessage"/>'s type<para></para><see cref="ApiServerMessageType">View default API server message types</see></param>
    /// <param name="sourceId">The unique identifier of the API server that has produced the <see cref="ApiServerMessage"/></param>
    /// <param name="content">The <see cref="ApiServerMessage"/>'s content, if any</param>
    public ApiServerMessage(string type, string sourceId, JsonObject? content = null)
    {
        this.Id = Guid.NewGuid().ToString();
        this.Type = type;
        this.CreatedAt = DateTimeOffset.Now;
        this.SourceId = sourceId;
        this.Content = content;
    }

    /// <summary>
    /// Gets the API server message's unique identifier
    /// </summary>
    [DataMember(Name = "id", Order = 1), JsonPropertyName("id")]
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Gets the API server message's type
    /// </summary>
    [DataMember(Name = "type", Order = 2), JsonPropertyName("type")]
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets the date and time at which the API server message has been created
    /// </summary>
    [DataMember(Name = "createdAt", Order = 3), JsonPropertyName("createdAt")]
    public virtual DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time at which the API server messagehas been received
    /// </summary>
    [DataMember(Name = "receivedAt", Order = 4), JsonPropertyName("receivedAt")]
    public virtual DateTimeOffset? ReceivedAt { get; set; }

    /// <summary>
    /// Gets the unique identifier of the API server that has produced the API server message
    /// </summary>
    [DataMember(Name = "sourceId", Order = 5), JsonPropertyName("sourceId")]
    public virtual string SourceId { get; set; } = null!;

    /// <summary>
    /// Gets the API server message's content, if any
    /// </summary>
    [DataMember(Name = "content", Order = 6), JsonPropertyName("content")]
    public virtual JsonObject? Content { get; set; }

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey, TValue}"/> that contains the API server message's extensions
    /// </summary>
    [DataMember(Name = "extensions", Order = 7), JsonExtensionData]
    public virtual IDictionary<string, object>? Extensions { get; set; }

}
