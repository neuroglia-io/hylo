namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the metadata used to describe a resource
/// </summary>
[DataContract]
public class V1ResourceMetadata
    : IExtensible
{

    /// <summary>
    /// Gets the resource's globally unique identifier
    /// </summary>
    [DataMember(Name = "id", Order = 1 ), JsonPropertyName("id")]
    public virtual string? Id { get; set; }

    /// <summary>
    /// Gets the resource's name. Ending the name with a '-' character will have the same effect than setting 'namePrefix' property
    /// </summary>
    [DataMember(Name = "name", Order = 2), JsonPropertyName("name"), Required]
    public virtual string? Name { get; set; } = null!;

    /// <summary>
    /// Gets the resource's generated name prefix. Setting this property will make Hylo generate the base-64 of a UUID prefixed by specified value
    /// </summary>
    [DataMember(Name = "namePrefix", Order = 3), JsonPropertyName("namePrefix"), Required]
    public virtual string? NamePrefix { get; set; }

    /// <summary>
    /// Gets the namespace the resource belongs to
    /// </summary>
    [DataMember(Name = "namespace", Order = 4), JsonPropertyName("namespace")]
    public virtual string? Namespace { get; set; }

    /// <summary>
    /// Gets the date and time at which the resource has been created
    /// </summary>
    [DataMember(Name = "createdAt", Order = 5), JsonPropertyName("createdAt")]
    public virtual DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time at which the resource has last been modified
    /// </summary>
    [DataMember(Name = "lastModified", Order = 6), JsonPropertyName("lastModified")]
    public virtual DateTimeOffset? LastModified { get; set; }

    /// <summary>
    /// Gets the resource's spec version
    /// </summary>
    [DataMember(Name = "stateVersion", Order = 7), JsonPropertyName("stateVersion")]
    public virtual long? StateVersion { get; set; }

    /// <summary>
    /// Gets mappings containing the resource's labels
    /// </summary>
    [DataMember(Name = "labels", Order = 8), JsonPropertyName("labels")]
    public virtual Dictionary<string, string>? Labels { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "extensions", Order = 9), JsonExtensionData]
    public IDictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// Generates a new name for the described resource
    /// </summary>
    /// <returns>The generated name, prefixed with the configured <see cref="NamePrefix"/></returns>
    public virtual string GenerateName()
    {
        return $"{this.NamePrefix}{Guid.NewGuid().ToShortString()}";
    }

}