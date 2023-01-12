namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure the names of a <see cref="V1ResourceDefinition"/>
/// </summary>
[DataContract]
public class V1ResourceDefinitionNames
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinitionNames"/>
    /// </summary>
    public V1ResourceDefinitionNames() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinitionNames"/>
    /// </summary>
    /// <param name="kind">The <see cref="V1ResourceDefinition"/>'s kind</param>
    /// <param name="singular">The singular form of the <see cref="V1ResourceDefinition"/>'s name</param>
    /// <param name="plural">The plural form of the <see cref="V1ResourceDefinition"/>'s name</param>
    /// <param name="shortNames">An <see cref="IEnumerable{T}"/> containing the <see cref="V1ResourceDefinition"/>'s short names</param>
    public V1ResourceDefinitionNames(string kind, string singular, string plural, IEnumerable<string>? shortNames)
    {
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        if (string.IsNullOrWhiteSpace(singular)) throw new ArgumentNullException(nameof(singular));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        this.Kind = kind;
        this.Singular = singular;
        this.Plural = plural;
        this.ShortNames = shortNames?.ToList();
    }

    /// <summary>
    /// Gets the <see cref="V1ResourceDefinition"/>'s kind
    /// </summary>
    [DataMember(Name = "kind", Order = 1), JsonPropertyName("kind"), Required]
    public virtual string Kind { get; set; } = null!;

    /// <summary>
    /// Gets the singular form of the <see cref="V1ResourceDefinition"/>'s name
    /// </summary>
    [DataMember(Name = "singular", Order = 2), JsonPropertyName("singular"), Required]
    public virtual string Singular { get; set; } = null!;

    /// <summary>
    /// Gets the plural form of the <see cref="V1ResourceDefinition"/>'s name
    /// </summary>
    [DataMember(Name = "plural", Order = 3), JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the <see cref="V1ResourceDefinition"/>'s short names
    /// </summary>
    [DataMember(Name = "shortNames", Order = 4), JsonPropertyName("shortNames")]
    public virtual List<string>? ShortNames { get; set; }

}
