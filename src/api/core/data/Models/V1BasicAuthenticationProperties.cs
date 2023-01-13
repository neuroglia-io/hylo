namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure a Basic authentication
/// </summary>
[DataContract]
public class V1BasicAuthenticationProperties
{

    /// <summary>
    /// Initializes a new <see cref="V1BasicAuthenticationProperties"/>
    /// </summary>
    public V1BasicAuthenticationProperties() { }

    /// <summary>
    /// Initializes a new <see cref="V1BasicAuthenticationProperties"/>
    /// </summary>
    /// <param name="passwordHash">The base-64 hash of the user's password</param>
    /// <param name="passwordSalt">The base-64 salt used to generate the user password's hash</param>
    public V1BasicAuthenticationProperties(string passwordHash, string passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentNullException(nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(passwordSalt)) throw new ArgumentNullException(nameof(passwordSalt));
        this.PasswordHash = passwordHash;
        this.PasswordSalt = passwordSalt;
    }

    /// <summary>
    /// Gets the base-64 hash of the user's password
    /// </summary>
    [DataMember(Name = "passwordHash", Order = 1), JsonPropertyName("passwordHash"), Required, MinLength(1)]
    public virtual string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets the base-64 salt used to generate the user password's hash
    /// </summary>
    [DataMember(Name = "passwordSalt", Order = 2), JsonPropertyName("passwordSalt"), Required, MinLength(1)]
    public virtual string PasswordSalt { get; set; } = null!;

}
