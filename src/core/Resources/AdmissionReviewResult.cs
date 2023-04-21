namespace Hylo;

/// <summary>
/// Represents an object used to describe the result of a resource admission review
/// </summary>
[DataContract]
public class AdmissionReviewResult
{

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewResult"/>
    /// </summary>
    public AdmissionReviewResult() { }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewResult"/>
    /// </summary>
    /// <param name="resource">The admitted resource, possibly mutated, resource</param>
    public AdmissionReviewResult(IResource resource)
    {
        this.Allowed = true;
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewResult"/>
    /// </summary>
    /// <param name="patch">The admitted resource, possibly mutated, resource</param>
    public AdmissionReviewResult(Patch patch)
    {
        this.Allowed = true;
        this.Patch = patch ?? throw new ArgumentNullException(nameof(patch));
    }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewResult"/>
    /// </summary>
    /// <param name="errors">A collection of errors that have occured during admission, if any, mapped by code</param>
    public AdmissionReviewResult(params KeyValuePair<string, string[]>[] errors)
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
    /// Gets/sets the admitted, possibly mutated, resource
    /// </summary>
    [DataMember(Order = 2, Name = "resource"), JsonPropertyOrder(2), JsonPropertyName("resource"), YamlMember(Order = 2, Alias = "resource")]
    public virtual IResource? Resource { get; set; }

    /// <summary>
    /// Gets/sets the admitted patch
    /// </summary>
    [DataMember(Order = 3, Name = "patch"), JsonPropertyOrder(3), JsonPropertyName("patch"), YamlMember(Order = 3, Alias = "patch")]
    public virtual Patch? Patch { get; set; }

    /// <summary>
    /// Gets/sets a collection of errors that have occured during admission, if any, mapped by code
    /// </summary>
    [DataMember(Order = 4, Name = "errors"), JsonPropertyOrder(4), JsonPropertyName("errors"), YamlMember(Order = 4, Alias = "errors")]
    public virtual IEnumerable<KeyValuePair<string, string[]>>? Errors { get; set; }

}
