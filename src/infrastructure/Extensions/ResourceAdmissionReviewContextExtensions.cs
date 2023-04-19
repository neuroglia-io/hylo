namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="ResourceAdmissionReviewContext"/>s
/// </summary>
public static class ResourceAdmissionReviewContextExtensions
{

    /// <summary>
    /// Converts the <see cref="ResourceAdmissionReviewContext"/> into a new <see cref="AdmissionReview"/>
    /// </summary>
    /// <param name="context">The <see cref="ResourceAdmissionReviewContext"/> to convert</param>
    /// <returns>A new <see cref="ResourceAdmissionReviewContext"/></returns>
    public static AdmissionReview ToAdmissionReview(this ResourceAdmissionReviewContext context) => new(new AdmissionReviewRequest(context.Id, context.Operation, context.Resource.ConvertTo<ResourceReference>()!, context.SubResource, context.UpdatedState, context.OriginalState, context.User));

}