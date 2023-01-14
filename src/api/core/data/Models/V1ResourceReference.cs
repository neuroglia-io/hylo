namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a reference to a <see cref="V1Resource"/>
/// </summary>
[DataContract]
public class V1ResourceReference
{

    public V1ResourceReference() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceReference"/>
    /// </summary>
    /// <param name="apiVersion">The API version of the referenced resource</param>
    /// <param name="kind">The referenced resource's kind</param>
    /// <param name="name">The referenced resource's name</param>
    /// <param name="namespace">The referenced resource's namespace</param>
    /// <exception cref="ArgumentNullException"></exception>
    public V1ResourceReference(string apiVersion, string kind, string name, string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(apiVersion)) throw new ArgumentNullException(nameof(apiVersion));
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.ApiVersion = apiVersion;
        this.Kind = kind;
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <summary>
    /// Gets/sets the API version of the referenced resource
    /// </summary>
    [DataMember(Name = "apiVersion", Order = 1), JsonPropertyName("apiVersion"), Required, MinLength(1)]
    public virtual string ApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets/sets the referenced resource's kind
    /// </summary>
    [DataMember(Name = "kind", Order = 2), JsonPropertyName("kind"), Required, MinLength(1)]
    public virtual string Kind { get; set; } = null!;

    /// <summary>
    /// Gets/sets the referenced resource's name
    /// </summary>
    [DataMember(Name = "name", Order = 3), JsonPropertyName("name"), Required, MinLength(1)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the referenced resource's namespace
    /// </summary>
    [DataMember(Name = "namespace", Order = 4), JsonPropertyName("namespace")]
    public virtual string? Namespace { get; set; }

    /// <inheritdoc/>
    public override string ToString() => string.IsNullOrWhiteSpace(this.Namespace) ? $"{this.ApiVersion}/{this.Kind}/{this.Name}" : $"{this.ApiVersion}/{this.Kind}/{this.Namespace}/{this.Name}";

}