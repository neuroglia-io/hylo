namespace Hylo;

/// <summary>
/// Represents a request to convert the version of a resource
/// </summary>
[DataContract]
public class ConversionReviewRequest
{

    /// <summary>
    /// Initializes a new <see cref="ConversionReviewRequest"/>
    /// </summary>
    public ConversionReviewRequest() { }

    /// <summary>
    /// Initializes a new <see cref="ConversionReviewRequest"/>
    /// </summary>
    /// <param name="uid">The globally unique identifier of the conversion request</param>
    /// <param name="desiredApiVersion">The version to convert the resource to</param>
    /// <param name="resource">The resource to convert</param>
    public ConversionReviewRequest(string uid, string desiredApiVersion, IResource resource)
    {
        if(string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
        if (string.IsNullOrWhiteSpace(desiredApiVersion)) throw new ArgumentNullException(nameof(desiredApiVersion));
        if (string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
        this.Uid = uid;
        this.DesiredApiVersion = desiredApiVersion;
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <summary>
    /// Gets the globally unique identifier of the conversion request
    /// </summary>
    [Required]
    [DataMember(Name = "uid", Order = 1, IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("uid"), YamlMember(Order = 1, Alias = "uid")]
    public virtual string Uid { get; set; } = null!;

    /// <summary>
    /// Gets the version to convert the resource to
    /// </summary>
    [Required]
    [DataMember(Name = "desiredApiVersion", Order = 2, IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("desiredApiVersion"), YamlMember(Order = 2, Alias = "desiredApiVersion")]
    public virtual string DesiredApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets the resource to convert
    /// </summary>
    [Required]
    [DataMember(Name = "resource", Order = 3, IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("resource"), YamlMember(Order = 3, Alias = "resource")]
    public virtual IResource Resource { get; set; } = null!;

}

