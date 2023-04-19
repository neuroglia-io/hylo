namespace Hylo;

/// <summary>
/// Represents an object used to describe the result of a resource admission review
/// </summary>
[DataContract]
public class ResourceAdmissionReviewResult
{

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionReviewResult"/>
    /// </summary>
    public ResourceAdmissionReviewResult() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionReviewResult"/>
    /// </summary>
    /// <param name="resource">The admitted resource</param>
    public ResourceAdmissionReviewResult(IResource resource)
    {
        this.Allowed = true;
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionReviewResult"/>
    /// </summary>
    /// <param name="errors"></param>
    public ResourceAdmissionReviewResult(params KeyValuePair<string, string[]>[] errors)
    {
        this.Allowed = false;
        this.Errors = errors;
    }

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the resource operation has been admitted
    /// </summary>
    [DataMember(Order = 1, Name = "allowed"), JsonPropertyOrder(1), JsonPropertyName("allowed"), YamlMember(Order = 1, Alias = "allowed")]
    public virtual bool Allowed { get; set; }

    /// <summary>
    /// Gets/sets the admitted resource
    /// </summary>
    [DataMember(Order = 2, Name = "resource"), JsonPropertyOrder(2), JsonPropertyName("resource"), YamlMember(Order = 2, Alias = "resource")]
    public virtual IResource? Resource { get; set; }

    /// <summary>
    /// Gets/sets a collection of errors that have occured during admission, if any, mapped by code
    /// </summary>
    [DataMember(Order = 3, Name = "errors"), JsonPropertyOrder(3), JsonPropertyName("errors"), YamlMember(Order = 3, Alias = "errors")]
    public virtual IEnumerable<KeyValuePair<string, string[]>>? Errors { get; set; }

}
