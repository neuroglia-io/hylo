namespace Hylo;

/// <summary>
/// Describes a patch
/// </summary>
[DataContract]
public record Patch
{

    /// <summary>
    /// Initializes a new <see cref="Patch"/>
    /// </summary>
    public Patch() { }

    /// <summary>
    /// Initializes a new <see cref="Patch"/>
    /// </summary>
    /// <param name="type">The type of patch to apply</param>
    /// <param name="document">The patch document</param>
    public Patch(PatchType type, object document)
    {
        this.Type = type;
        this.Document = document;
    }

    /// <summary>
    /// Gets/sets the patch's type
    /// </summary>
    [DataMember(Order = 1, Name = "type"), JsonPropertyOrder(1), JsonPropertyName("type"), YamlMember(Order = 1, Alias = "type")]
    public virtual PatchType Type { get; set; }

    /// <summary>
    /// Gets/sets the patch document
    /// </summary>
    [Required, JsonRequired]
    [DataMember(Order = 2, Name = "document", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("document"), YamlMember(Order = 2, Alias = "document")]
    public virtual object Document { get; set; } = null!;

}