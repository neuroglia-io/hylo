namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="AdmissionReviewContext"/>s
/// </summary>
public static class AdmissionReviewContextExtensions
{

    /// <summary>
    /// Converts the <see cref="AdmissionReviewContext"/> into a new <see cref="AdmissionReview"/>
    /// </summary>
    /// <param name="context">The <see cref="AdmissionReviewContext"/> to convert</param>
    /// <returns>A new <see cref="AdmissionReviewContext"/></returns>
    public static AdmissionReview ToAdmissionReview(this AdmissionReviewContext context) => new(new AdmissionReviewRequest(context.Id, context.Operation, context.Resource.ConvertTo<ResourceReference>()!, context.SubResource, context.UpdatedState, context.OriginalState, context.User));

}