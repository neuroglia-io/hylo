namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure a <see cref="V1UserAccount"/>
/// </summary>
[DataContract]
public class V1UserAccountSpec
{

    /// <summary>
    /// Initializes a new <see cref="V1UserAccountSpec"/>
    /// </summary>
    public V1UserAccountSpec() { }

    /// <summary>
    /// Initializes a new <see cref="V1UserAccountSpec"/>
    /// </summary>
    /// <param name="authentication">The object used to configure the user account's authentication methods</param>
    public V1UserAccountSpec(V1UserAccountAuthentication authentication)
    {
        this.Authentication = authentication;
    }

    /// <summary>
    /// Gets/sets the object used to configure the user account's authentication methods
    /// </summary>
    [DataMember(Name = "authentication", Order = 1), JsonPropertyName("authentication"), Required]
    public virtual V1UserAccountAuthentication Authentication { get; set; } = null!;

}
