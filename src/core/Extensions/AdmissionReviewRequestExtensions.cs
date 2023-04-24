namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="AdmissionReviewRequest"/>s
/// </summary>
public static class AdmissionReviewRequestExtensions
{

    /// <summary>
    /// Creates a new <see cref="Patch"/> based on difference between the updated state and the original state of the admitted resource
    /// </summary>
    /// <param name="request">The extended <see cref="IResource"/></param>
    /// <returns>A new <see cref="Patch"/></returns>
    public static Patch GetDiffPatch(this AdmissionReviewRequest request)
    {
        if(request == null) throw new ArgumentNullException(nameof(request));
        return new(PatchType.JsonPatch, JsonPatchHelper.CreateJsonPatchFromDiff(request.OriginalState, request.UpdatedState));
    }

}
