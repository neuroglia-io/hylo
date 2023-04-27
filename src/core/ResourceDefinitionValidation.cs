namespace Hylo;

/// <summary>
/// Represents the object used to configure a schema to validate defined resources
/// </summary>
[DataContract]
public record ResourceDefinitionValidation
{

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionValidation"/>
    /// </summary>
    public ResourceDefinitionValidation() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionValidation"/>
    /// </summary>
    /// <param name="openAPIV3Schema">The JSON schema used to validate defined resources</param>
    public ResourceDefinitionValidation(JsonSchema openAPIV3Schema)
    {
        this.OpenAPIV3Schema = openAPIV3Schema;
    }

    /// <summary>
    /// Gets/sets the JSON schema used to validate defined resources
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "openAPIV3Schema", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("openAPIV3Schema"), YamlMember(Order = 1, Alias = "openAPIV3Schema")]
    public virtual JsonSchema OpenAPIV3Schema { get; set; } = null!;

}