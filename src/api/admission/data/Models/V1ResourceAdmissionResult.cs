namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Describes the result of a <see cref="V1Resource"/>'s admission
/// </summary>
[DataContract]
public class V1ResourceAdmissionResult
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceAdmissionResult"/>
    /// </summary>
    public V1ResourceAdmissionResult() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceAdmissionResult"/>
    /// </summary>
    /// <param name="succeeded">A boolean indicating whether or not the admission evaluation succeeded</param>
    /// <param name="patch">The patch to apply if the mutation succeeded. Used only when mutating, ignored in all other contexts</param>
    /// <param name="errors"></param>
    public V1ResourceAdmissionResult(bool succeeded, V1Patch? patch = null, IEnumerable<V1Error>? errors = null)
    {
        this.Succeeded = succeeded;
        this.Errors = errors?.ToList();
        this.Patch = patch;
    }

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the admission evaluation succeeded
    /// </summary>
    [DataMember(Name = "succeeded", Order = 1), JsonPropertyName("succeeded")]
    public virtual bool Succeeded { get; set; }

    /// <summary>
    /// Gets/sets the patch to apply if the mutation succeeded. Used only when mutating, ignored in all other contexts
    /// </summary>
    [DataMember(Name = "patch", Order = 2), JsonPropertyName("patch")]
    public virtual V1Patch? Patch { get; set; }

    /// <summary>
    /// Gets/sets a list containing the errors that have occured during the resource's admission evaluation
    /// </summary>
    [DataMember(Name = "errors", Order = 3), JsonPropertyName("errors")]
    public virtual List<V1Error>? Errors { get; set; }

}
