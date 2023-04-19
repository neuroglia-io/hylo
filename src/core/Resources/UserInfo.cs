namespace Hylo.Resources;

/// <summary>
/// Represents an object that holds information about a user
/// </summary>
[DataContract]
public class UserInfo
{

    /// <summary>
    /// Initializes a new <see cref="UserInfo"/>
    /// </summary>
    public UserInfo() { }

    /// <summary>
    /// Initializes a new <see cref="UserInfo"/>
    /// </summary>
    /// <param name="name">The name of the user to describe</param>
    /// <param name="authenticationType">The name of the mechanism used to authenticate the user</param>
    /// <param name="claims">A key/comma-separated values mapping of the claims used to describe the authenticated user</param>
    public UserInfo(string name, string authenticationType, IDictionary<string, string>? claims = null)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(authenticationType)) throw new ArgumentNullException(nameof(authenticationType));
        this.Name = name;
        this.AuthenticationType = authenticationType;
        this.Claims = claims;
    }

    /// <summary>
    /// Gets/sets the name of the user to describe
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "name", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("name"), YamlMember(Order = 1, Alias = "name")]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the name of the mechanism used to authenticate the user
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "authenticationType", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("authenticationType"), YamlMember(Order = 2, Alias = "authenticationType")]
    public virtual string AuthenticationType { get; set; } = null!;

    /// <summary>
    /// Gets/sets a key/comma-separated values mapping of the claims used to describe the authenticated user
    /// </summary>
    [DataMember(Order = 3, Name = "authenticationType", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("authenticationType"), YamlMember(Order = 3, Alias = "authenticationType")]
    public virtual IDictionary<string, string>? Claims { get; set; }

}
