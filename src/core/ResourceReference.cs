namespace Hylo;

/// <summary>
/// Represents an object used to reference a resource
/// </summary>
[DataContract]
public class ResourceReference
    : IResourceReference
{

    /// <summary>
    /// Initializes a new <see cref="ResourceReference"/>
    /// </summary>
    public ResourceReference() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceReference"/>
    /// </summary>
    /// <param name="definition">The referenced resource's definition</param>
    /// <param name="name">The name of the referenced resource</param>
    /// <param name="namespace">The namespace the referenced resource belongs to, if any</param>
    public ResourceReference(ResourceDefinitionReference definition, string name, string? @namespace = null)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 1, Name = "uid", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("uid"), YamlMember(Order = 1, Alias = "uid")]
    public virtual ResourceDefinitionReference Definition { get; set; } = null!;

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 2, Name = "name", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("name"), YamlMember(Order = 2, Alias = "name")]
    public virtual string Name { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Order = 3, Name = "namespace"), JsonPropertyOrder(3), JsonPropertyName("namespace"), YamlMember(Order = 3, Alias = "namespace")]
    public virtual string? Namespace { get; set; }

    IResourceDefinitionReference IResourceReference.Definition => this.Definition;

    /// <inheritdoc/>
    public override string ToString() => string.IsNullOrWhiteSpace(this.Namespace) ? $"{this.Definition}/{this.Name}" : $"{this.Definition.Group}/{this.Definition.Version}/namespaces/{this.Namespace}/{this.Definition.Plural}/{this.Name}";

}
