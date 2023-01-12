namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Describes an error
/// </summary>
[DataContract]
public class V1Error
    : IExtensible
{

    /// <summary>
    /// Initializes a new <see cref="V1Error"/>
    /// </summary>
    public V1Error() { }

    /// <summary>
    /// Initializes a new <see cref="V1Error"/>
    /// </summary>
    /// <param name="code">The <see cref="V1Error"/>'s code</param>
    /// <param name="message">The <see cref="V1Error"/>'s message</param>
    public V1Error(string code, string? message)
    {
        this.Code = code;
        this.Message = message;
    }

    /// <summary>
    /// Gets the <see cref="V1Error"/>'s code
    /// </summary>
    [DataMember(Name = "code", Order = 1), JsonPropertyName("code")]
    public virtual string Code { get; set; } = null!;

    /// <summary>
    /// Gets the <see cref="V1Error"/>'s message
    /// </summary>
    [DataMember(Name = "message", Order = 2), JsonPropertyName("message")]
    public virtual string? Message { get; set; } = null!;

    /// <summary>
    /// Gets the <see cref="V1Error"/>'s metadata, if any
    /// </summary>
    [DataMember(Name = "extensions", Order = 3), JsonExtensionData]
    public virtual IDictionary<string, object>? Extensions { get; set; }

    /// <inheritdoc/>
    public override string ToString() => $"{this.Code}: {this.Message}";

}