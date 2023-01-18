namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a <see cref="V1Resource"/> version conversion request
/// </summary>
[DataContract]
public class V1ConversionReviewRequest
{

    /// <summary>
    /// Initializes a new <see cref="V1ConversionReviewRequest"/>
    /// </summary>
    public V1ConversionReviewRequest() { }

    /// <summary>
    /// Initializes a new <see cref="V1ConversionReviewRequest"/>
    /// </summary>
    /// <param name="desiredApiVersion">The version to convert the <see cref="V1Resource"/> to</param>
    /// <param name="resource">The resource to convert</param>
    public V1ConversionReviewRequest(string desiredApiVersion, object resource)
    {
        this.Id = Guid.NewGuid().ToString();
        this.DesiredApiVersion = desiredApiVersion;
        this.Resource = resource;
    }

    /// <summary>
    /// Gets the id of the <see cref="V1ResourceAdmissionRequest"/>
    /// </summary>
    [DataMember(Name = "id", Order = 1), JsonPropertyName("id"), Required]
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Gets the version to convert the <see cref="V1Resource"/> to
    /// </summary>
    [DataMember(Name = "desiredApiVersion", Order = 1), JsonPropertyName("desiredApiVersion"), Required]
    public virtual string DesiredApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets the resource to convert
    /// </summary>
    [DataMember(Name = "resource", Order = 1), JsonPropertyName("resource"), Required]
    public virtual object Resource { get; set; } = null!;

}

