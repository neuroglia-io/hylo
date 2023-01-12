namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure a version of a <see cref="V1ResourceDefinition"/>
/// </summary>
[DataContract]
public class V1ResourceDefinitionVersion
{

    /// <summary>
    /// Gets the <see cref="V1ResourceDefinitionVersion"/>'s name
    /// </summary>
    [DataMember(Name = "name", Order = 1), JsonPropertyName("name"), Required]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets a boolean indicating whether or not the <see cref="V1ResourceDefinitionVersion"/> is served by the API
    /// </summary>
    [DataMember(Name = "served", Order = 2), JsonPropertyName("served")]
    public virtual bool Served { get; set; }

    /// <summary>
    /// Gets a boolean indicating whether or not the <see cref="V1ResourceDefinitionVersion"/> is used for storage
    /// </summary>
    [DataMember(Name = "storage", Order = 3), JsonPropertyName("storage")]
    public virtual bool Storage { get; set; }

    /// <summary>
    /// Gets the <see cref="JsonSchema"/> used to validate <see cref="V1Resource"/>s based on the <see cref="V1ResourceDefinition"/>
    /// </summary>
    [DataMember(Name = "schema", Order = 4), JsonPropertyName("schema")]
    public virtual JsonSchema? Schema { get; set; }

    /// <inheritdoc/>
    public override string ToString() => this.Name;

}