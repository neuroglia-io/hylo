namespace Hylo;

/// <summary>
/// Defines the fundamentals of a reference to a resource definition
/// </summary>
public interface IResourceDefinitionReference
{

    /// <summary>
    /// Gets the API group resources of the described definition belong to
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "group", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("group"), YamlMember(Order = 1, Alias = "group")]
    string Group { get; }

    /// <summary>
    /// Gets the version of the API group resources of the described definition belong to
    /// </summary>
    [Required]
    [DataMember(Order = -2, Name = "version", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("version"), YamlMember(Order = 2, Alias = "version")]
    string Version { get; }

    /// <summary>
    /// Gets the plural name resources of the described definition belong to
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "plural", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("plural"), YamlMember(Order = 3, Alias = "plural")]
    string Plural { get; }

}
