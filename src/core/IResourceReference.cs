namespace Hylo;

/// <summary>
/// Defines the fundamentals of a reference to a resource
/// </summary>
public interface IResourceReference
{

    /// <summary>
    /// Gets the referenced resource's definition
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "definition", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("definition"), YamlMember(Order = 1, Alias = "definition")]
    IResourceDefinitionReference Definition { get; }

    /// <summary>
    /// Gets the name of the referenced resource
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "name", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("name"), YamlMember(Order = 2, Alias = "name")]
    string Name { get; }

    /// <summary>
    /// Gets the namespace the referenced resource belongs to
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "namespace", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("namespace"), YamlMember(Order = 3, Alias = "namespace")]
    string? Namespace { get; }

}
