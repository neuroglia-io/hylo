namespace Hylo;

/// <summary>
/// Represents an object used to filter <see cref="IResource"/>s
/// </summary>
[DataContract]
public record RuleWithOperation
{

    /// <summary>
    /// Initializes a new <see cref="RuleWithOperation"/>
    /// </summary>
    public RuleWithOperation() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scope">An expression used to filter <see cref="IResource"/>s by scope</param>
    /// <param name="apiGroups">A <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by api groups</param>
    /// <param name="apiVersions">A <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by api versions</param>
    /// <param name="kinds">A <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by kind</param>
    /// <param name="operations">A <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by operations</param>
    public RuleWithOperation(string? scope, List<string>? apiGroups, List<string>? apiVersions, List<string>? kinds, List<string>? operations)
    {
        this.Scope = scope;
        this.ApiGroups = apiGroups?.WithValueSemantics();
        this.ApiVersions = apiVersions?.WithValueSemantics();
        this.Kinds = kinds?.WithValueSemantics();
        this.Operations = operations?.WithValueSemantics();
    }

    /// <summary>
    /// Gets an expression used to filter <see cref="IResource"/>s by scope
    /// </summary>
    [DataMember(Name = "scope", Order = 1), JsonPropertyOrder(1), JsonPropertyName("scope"), YamlMember(Order = 1, Alias = "scope")]
    public virtual string? Scope { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by api groups
    /// </summary>
    [DataMember(Name = "apiGroups", Order = 2), JsonPropertyOrder(2), JsonPropertyName("apiGroups"), YamlMember(Order = 2, Alias = "apiGroups")]
    public virtual EquatableList<string>? ApiGroups { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by api versions
    /// </summary>
    [DataMember(Name = "apiVersions", Order = 3), JsonPropertyOrder(3), JsonPropertyName("apiVersions"), YamlMember(Order = 3, Alias = "apiVersions")]
    public virtual EquatableList<string>? ApiVersions { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by kind
    /// </summary>
    [DataMember(Name = "kinds", Order = 4), JsonPropertyOrder(4), JsonPropertyName("kinds"), YamlMember(Order = 4, Alias = "kinds")]
    public virtual EquatableList<string>? Kinds { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="IResource"/>s by operations
    /// </summary>
    [DataMember(Name = "operations", Order = 5), JsonPropertyOrder(5), JsonPropertyName("operations"), YamlMember(Order = 5, Alias = "operations")]
    public virtual EquatableList<string>? Operations { get; set; }

}