namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a reference to a <see cref="V1Resource"/>
/// </summary>
[DataContract]
public class V1ResourceReference
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceReference"/>
    /// </summary>
    public V1ResourceReference() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceReference"/>
    /// </summary>
    /// <param name="group">The <see cref="V1Resource"/>'s API group</param>
    /// <param name="version">The <see cref="V1Resource"/>'s API version</param>
    /// <param name="plural">The plural form of the <see cref="V1Resource"/>'s name</param>
    /// <param name="name">The <see cref="V1Resource"/>'s name</param>
    /// <param name="namespace">The <see cref="V1Resource"/>'s namespace</param>
    public V1ResourceReference(string group, string version, string plural, string? name = null, string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <summary>
    /// Gets the <see cref="V1Resource"/>'s API group
    /// </summary>
    [DataMember(Name = "group", Order = 1), JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the <see cref="V1Resource"/>'s API version
    /// </summary>
    [DataMember(Name = "version", Order = 2), JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the plural form of the <see cref="V1Resource"/>'s name
    /// </summary>
    [DataMember(Name = "plural", Order = 3), JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets the <see cref="V1Resource"/>'s name
    /// </summary>
    [DataMember(Name = "name", Order = 4), JsonPropertyName("name"), Required]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets the <see cref="V1Resource"/>'s namespace
    /// </summary>
    [DataMember(Name = "namespace", Order = 5), JsonPropertyName("namespace")]
    public virtual string? Namespace { get; set; }

    /// <summary>
    /// Gets the API version of the referenced <see cref="V1Resource"/>
    /// </summary>
    /// <returns>The API version of the referenced <see cref="V1Resource"/></returns>
    public virtual string GetApiVersion() => ApiVersion.Build(this.Group, this.Version);

    /// <inheritdoc/>
    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(this.Namespace)) return $"{this.Group}/{this.Version}/{this.Plural}/{this.Name}";
        else return $"{this.Group}/{this.Version}/{this.Plural}/{this.Namespace}/{this.Name}";
    }

    /// <summary>
    /// Implicitly converts the specified <see cref="V1Resource"/> into a new <see cref="V1ResourceReference"/>
    /// </summary>
    /// <returns>A new <see cref="V1ResourceReference"/></returns>
    public static implicit operator V1ResourceReference?(V1Resource? resource) => CreateFor(resource);

    /// <summary>
    /// Creates a new <see cref="V1ResourceReference"/> for the specified <see cref="V1Resource"/>
    /// </summary>
    /// <param name="resource">The <see cref="V1Resource"/> to create a new <see cref="V1ResourceReference"/> for</param>
    /// <returns>A new <see cref="V1ResourceReference"/></returns>
    public static V1ResourceReference? CreateFor(V1Resource? resource)
    {
        if(resource == null) return null;
        else return new(ApiVersion.GetGroup(resource.ApiVersion), ApiVersion.GetVersion(resource.ApiVersion), resource.Kind, resource.Metadata.Name!, resource.Metadata.Namespace);
    }


}
