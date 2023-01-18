namespace Hylo.Api.Admission.Data.Models;

/// <summary>
/// Represents the
/// </summary>
[DataContract]
public class V1AdmissionReviewResponse
{

    /// <summary>
    /// Initializes a new <see cref="V1AdmissionReviewResponse"/>
    /// </summary>
    public V1AdmissionReviewResponse() { }

    /// <summary>
    /// Initializes a new <see cref="V1AdmissionReviewResponse"/>
    /// </summary>
    /// <param name="id">The id of the <see cref="V1AdmissionReviewRequest"/> answered to by the <see cref="V1AdmissionReviewResponse"/></param>
    /// <param name="succeeded">A boolean indicating whether or not the admission succeeded</param>
    /// <param name="patch">The <see cref="V1Patch"/> applied during the resource's admission, if any</param>
    /// <param name="errors">A <see cref="List{T}"/> containing the <see cref="V1Error"/>s that occured during the resource's admission</param>
    public V1AdmissionReviewResponse(string id, bool succeeded, V1Patch? patch = null, IEnumerable<V1Error>? errors = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
        this.Id = id;
        this.Succeeded = succeeded;
        this.Errors = errors?.ToList();
        this.Patch = patch;
    }

    /// <summary>
    /// Gets the id of the request the response describes the result of
    /// </summary>
    [DataMember(Name = "id", Order = 1), JsonPropertyName("id")]
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Gets a boolean indicating whether or not the admission succeeded
    /// </summary>
    [DataMember(Name = "succeeded", Order = 2), JsonPropertyName("succeeded")]
    public virtual bool Succeeded { get; set; }

    /// <summary>
    /// Gets the patch  applied during the resource's admission, if any
    /// </summary>
    [DataMember(Name = "patch", Order = 3), JsonPropertyName("patch")]
    public virtual V1Patch? Patch { get; set; }

    /// <summary>
    /// Gets a list containing the errors that occured during the resource's admission
    /// </summary>
    [DataMember(Name = "errors", Order = 4), JsonPropertyName("errors")]
    public virtual List<V1Error>? Errors { get; set; }

}