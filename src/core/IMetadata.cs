namespace Hylo;

/// <summary>
/// Defines the fundamentals of an object described by metadata
/// </summary>
public interface IMetadata
{

    /// <summary>
    /// Gets the metadata that describes the object
    /// </summary>
    [Required]
    [DataMember(Order = -997, Name = "metadata", IsRequired = true), JsonPropertyOrder(-997), JsonPropertyName("metadata"), YamlMember(Order = -997, Alias = "metadata")]
    object Metadata { get; }

}

/// <summary>
/// Defines the fundamentals of an object described by metadata
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata</typeparam>
public interface IMetadata<TMetadata>
    : IMetadata
    where TMetadata : class, new()
{

    /// <summary>
    /// Gets the metadata that describes the object
    /// </summary>
    new TMetadata Metadata { get; }

}
