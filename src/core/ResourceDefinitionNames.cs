namespace Hylo;

/// <summary>
/// Represents an object used to configure the names of a resource definition
/// </summary>
[DataContract]
public record ResourceDefinitionNames
{

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionNames"/>
    /// </summary>
    public ResourceDefinitionNames() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinitionNames"/>
    /// </summary>
    /// <param name="singular">The singular form of the resource definition's name</param>
    /// <param name="plural">The plural form of the resource definition's name</param>
    /// <param name="kind">The resource definition's kind</param>
    /// <param name="shortNames">A collection containing the resource definition's short names, if any</param>
    public ResourceDefinitionNames(string singular, string plural, string kind, IEnumerable<string>? shortNames = null)
    {
        if (string.IsNullOrWhiteSpace(singular)) throw new ArgumentNullException(nameof(singular));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        this.Singular = singular;
        this.Plural = plural;
        this.Kind = kind;
        this.ShortNames = shortNames?.WithValueSemantics();
    }

    /// <summary>
    /// Gets/sets the singular form of the resource definition's name
    /// </summary>
    [Required, MinLength(3)]
    [DataMember(Order = 1, Name = "singular", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("singular"), YamlMember(Order = 1, Alias = "singular")]
    public virtual string Singular { get; set; } = null!;

    /// <summary>
    /// Gets/sets the plural form of the resource definition's name
    /// </summary>
    [Required, MinLength(3)]
    [DataMember(Order = 2, Name = "plural", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("plural"), YamlMember(Order = 2, Alias = "plural")]
    public virtual string Plural { get; set; } = null!;

    /// <summary>
    /// Gets/sets the resource definition's kind
    /// </summary>
    [Required, MinLength(3)]
    [DataMember(Order = 3, Name = "kind", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("kind"), YamlMember(Order = 3, Alias = "kind")]
    public virtual string Kind { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the resource definition's short names, if any
    /// </summary>
    [Required, MinLength(1)]
    [DataMember(Order = 4, Name = "shortNames", IsRequired = true), JsonPropertyOrder(4), JsonPropertyName("shortNames"), YamlMember(Order = 4, Alias = "shortNames")]
    public virtual EquatableList<string>? ShortNames { get; set; }

}
