namespace Hylo;

/// <summary>
/// Represents a resource version conversion review
/// </summary>
[DataContract]
public class ConversionReview
    : IObject
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public static string ResourceGroup { get; set; } = HyloDefaults.ResourceGroup;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public static string ResourceVersion { get; set; } = "v1";
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public static string ResourceKind { get; set; } = "ConversionReview";

    /// <summary>
    /// Initializes a new <see cref="ConversionReview"/>
    /// </summary>
    public ConversionReview() { }

    /// <summary>
    /// Initializes a new <see cref="ConversionReview"/>
    /// </summary>
    /// <param name="request">The <see cref="ConversionReview"/>'s request</param>
    public ConversionReview(ConversionReviewRequest request) 
        : this()
    {
        this.Request = request;
    }

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 1, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("apiVersion"), YamlMember(Order = 1, Alias = "apiVersion")]
    public virtual string ApiVersion { get; set; } = Hylo.ApiVersion.Build(ResourceGroup, ResourceVersion);

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 2, Name = "kind", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("kind"), YamlMember(Order = 2, Alias = "kind")]
    public virtual string Kind { get; set; } = ResourceKind;

    /// <summary>
    /// Gets the review's request
    /// </summary>
    [DataMember(Name = "request", Order = 3), JsonPropertyOrder(3), JsonPropertyName("request"), YamlMember(Order = 3, Alias = "request")]
    public virtual ConversionReviewRequest Request { get; set; } = null!;

    /// <summary>
    /// Gets the review's response
    /// </summary>
    [DataMember(Name = "response", Order = 4), JsonPropertyOrder(4), JsonPropertyName("response"), YamlMember(Order = 4, Alias = "response")]
    public virtual ConversionReviewResponse? Response { get; set; }

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

}
