namespace Hylo;

/// <summary>
/// Represents an object used to reference a sub resource
/// </summary>
[DataContract]
public record SubResourceReference
    : ResourceReference, ISubResourceReference
{

    /// <summary>
    /// Initializes a new <see cref="SubResourceReference"/>
    /// </summary>
    public SubResourceReference() { }

    /// <summary>
    /// Initializes a new <see cref="SubResourceReference"/>
    /// </summary>
    /// <param name="definition">The referenced resource's definition</param>
    /// <param name="name">The name of the referenced resource</param>
    /// <param name="subResource">The name of the referenced sub resource</param>
    /// <param name="namespace">The namespace the referenced resource belongs to, if any</param>
    public SubResourceReference(ResourceDefinitionReference definition, string name, string subResource, string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(subResource)) throw new ArgumentNullException(nameof(subResource));
        this.Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 1, Name = "subResource", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("subResource"), YamlMember(Order = 1, Alias = "subResource")]
    public virtual string SubResource { get; set; } = null!;

    /// <inheritdoc/>
    public override string ToString() => $"{base.ToString()}/{this.SubResource}";

}