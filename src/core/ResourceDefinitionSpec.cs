namespace Hylo;

/// <summary>
/// Represents an object used to configure a resource definition
/// </summary>
[DataContract]
public class ResourceDefinitionSpec
{

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionSpec"/>
    /// </summary>
    public ResourceDefinitionSpec() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionSpec"/>
    /// </summary>
    /// <param name="scope">The resource definition's scope</param>
    /// <param name="group">The resource definition's group</param>
    /// <param name="names">An object used to configure the resource definition's names</param>
    /// <param name="versions">A list containing object used to describe the resource definition's versions</param>
    public ResourceDefinitionSpec(ResourceScope scope, string group, ResourceDefinitionNames names, params ResourceDefinitionVersion[] versions)
    {
        if (versions == null) throw new ArgumentNullException(nameof(versions));
        if(versions.Length < 1) throw new ArgumentOutOfRangeException(nameof(versions));
        this.Scope = scope;
        this.Group = group;
        this.Names = names ?? throw new ArgumentNullException(nameof(names));
        this.Versions = versions.ToList();
    }

    /// <summary>
    /// Gets/sets the resource definition's scope
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "scope", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("scope"), YamlMember(Order = 1, Alias = "scope")]
    public virtual ResourceScope Scope { get; set; }

    /// <summary>
    /// Gets/sets the resource definition's group
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "group", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("group"), YamlMember(Order = 2, Alias = "group")]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets/sets an object used to configure the resource definition's names
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "names", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("names"), YamlMember(Order = 3, Alias = "names")]
    public virtual ResourceDefinitionNames Names { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing object used to describe the resource definition's versions
    /// </summary>
    [Required, MinLength(1)]
    [DataMember(Order = 4, Name = "versions", IsRequired = true), JsonPropertyOrder(4), JsonPropertyName("versions"), YamlMember(Order = 4, Alias = "versions")]
    public virtual List<ResourceDefinitionVersion> Versions { get; set; } = null!;

    /// <summary>
    /// Gets an object used to configure the way versions of the resource definition should be converted
    /// </summary>
    [DataMember(Name = "conversion", Order = 5), JsonPropertyOrder(5), JsonPropertyName("conversion"), YamlMember(Order = 5, Alias = "conversion")]
    public virtual ResourceConversion? Conversion { get; set; }

}
