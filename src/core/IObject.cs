namespace Hylo;

/// <summary>
/// Defines the fundamentals of a stored object
/// </summary>
public interface IObject
    : IExtensible
{

    /// <summary>
    /// Gets the API version that defines the versioned group the object belongs to
    /// </summary>
    [Required]
    [DataMember(Order = -999, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(-999), JsonPropertyName("apiVersion"), YamlMember(Order = -999, Alias = "apiVersion")]
    string ApiVersion { get; }

    /// <summary>
    /// Gets the object's kind
    /// </summary>
    [Required]
    [DataMember(Order = -998, Name = "kind", IsRequired = true), JsonPropertyOrder(-998), JsonPropertyName("kind"), YamlMember(Order = -998, Alias = "kind")]
    string Kind { get; }

}