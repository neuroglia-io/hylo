namespace Hylo.Api;

/// <summary>
/// Represents an object used to filter <see cref="V1Resource"/>s
/// </summary>
[DataContract]
public class V1RuleWithOperation
{

    /// <summary>
    /// Gets an expression used to filter <see cref="V1Resource"/>s by scope
    /// </summary>
    [DataMember(Name = "scope", Order = 1), JsonPropertyName("scope")]
    public virtual string? Scope { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="V1Resource"/>s by api groups
    /// </summary>
    [DataMember(Name = "apiGroups", Order = 2), JsonPropertyName("apiGroups")]
    public virtual List<string>? ApiGroups { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="V1Resource"/>s by api versions
    /// </summary>
    [DataMember(Name = "apiVersions", Order = 3), JsonPropertyName("apiVersions")]
    public virtual List<string>? ApiVersions { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="V1Resource"/>s by kind
    /// </summary>
    [DataMember(Name = "kinds", Order = 4), JsonPropertyName("kinds")]
    public virtual List<string>? Kinds { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing expressions used to filter <see cref="V1Resource"/>s by operations
    /// </summary>
    [DataMember(Name = "operations", Order = 5), JsonPropertyName("operations")]
    public virtual List<string>? Operations { get; set; }

}