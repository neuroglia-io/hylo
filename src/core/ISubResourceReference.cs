namespace Hylo;

/// <summary>
/// Defines the fundamentals of a reference to a sub resource
/// </summary>
public interface ISubResourceReference
    : IResourceReference
{


    /// <summary>
    /// Gets the name of the referenced sub resource
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "subResource", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("subResource"), YamlMember(Order = 1, Alias = "subResource")]
    string SubResource { get; }

}