namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a <see cref="V1ResourceDefinition"/>'s specification
/// </summary>
[DataContract]
public class V1ResourceDefinitionSpec
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinitionSpec"/>
    /// </summary>
    public V1ResourceDefinitionSpec() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinitionSpec"/>
    /// </summary>
    /// <param name="names">An object used to configure the <see cref="V1ResourceDefinition"/>'s names</param>
    /// <param name="group">The API group the <see cref="V1ResourceDefinition"/> belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1ResourceDefinition"/> belongs to</param>
    /// <param name="scope">The scope of defined <see cref="V1Resource"/>s</param>
    /// <param name="versions">An <see cref="IEnumerable{T}"/> containing the supported versions of the <see cref="V1ResourceDefinition"/></param>
    /// <param name="conversion">An object used to configure the way versions of the <see cref="V1ResourceDefinition"/> should be converted</param>
    public V1ResourceDefinitionSpec(V1ResourceDefinitionNames names, string group, string version, string scope, IEnumerable<V1ResourceDefinitionVersion> versions, V1ResourceConversionSpec? conversion = null)
    {
        if (names == null) throw new ArgumentNullException(nameof(names));
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(scope)) throw new ArgumentNullException(nameof(scope));
        if (versions == null) throw new ArgumentNullException(nameof(versions));
        if (!versions.Any()) throw new ArgumentOutOfRangeException(nameof(versions));
        if (versions.SingleOrDefault(v => v.Served) == null) throw new NullReferenceException();
        this.Names = names;
        this.Group = group;
        this.Version = version;
        this.Scope = scope;
        this.Versions = versions.ToList();
        this.Conversion = conversion;
    }

    /// <summary>
    /// Gets an object used to configure the resource definition's names
    /// </summary>
    [DataMember(Name = "names", Order = 1), JsonPropertyName("names"), Required]
    public virtual V1ResourceDefinitionNames Names { get; set; } = null!;

    /// <summary>
    /// Gets the API group the resource definition belongs to
    /// </summary>
    [DataMember(Name = "group", Order = 2), JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the resource definition belongs to
    /// </summary>
    [DataMember(Name = "version", Order = 3), JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the scope of defined resources
    /// </summary>
    [DataMember(Name = "scope", Order = 4), JsonPropertyName("scope"), Required]
    public virtual string Scope { get; set; } = null!;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the supported versions of the resource definition
    /// </summary>
    [DataMember(Name = "versions", Order = 5), JsonPropertyName("versions"), Required, MinLength(1)]
    public virtual List<V1ResourceDefinitionVersion> Versions { get; set; } = null!;

    /// <summary>
    /// Gets an object used to configure the way versions of the resource definition should be converted
    /// </summary>
    [DataMember(Name = "conversion", Order = 6), JsonPropertyName("conversion")]
    public virtual V1ResourceConversionSpec? Conversion { get; set; }

}
