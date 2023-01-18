namespace Hylo.Api.Admission.Data.Models;

/// <summary>
/// Represents a <see cref="V1Resource"/> admission review request
/// </summary>
[DataContract]
public class V1AdmissionReviewRequest
{

    /// <summary>
    /// Initializes a new <see cref="V1AdmissionReviewRequest"/>
    /// </summary>
    public V1AdmissionReviewRequest() { }

    /// <summary>
    /// Initializes a new <see cref="V1AdmissionReviewRequest"/>
    /// </summary>
    /// <param name="resource">The resource to validate</param>
    public V1AdmissionReviewRequest(object resource)
    {
        this.Id = Guid.NewGuid().ToString();
        this.Resource = resource;
    }

    /// <summary>
    /// Gets the request's id
    /// </summary>
    [DataMember(Name = "id", Order = 1), JsonPropertyName("id"), Required]
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Gets the resource to validate
    /// </summary>
    [DataMember(Name = "resource", Order = 2), JsonPropertyName("resource"), Required]
    public virtual object Resource { get; set; } = null!;

}
