namespace Hylo.Infrastructure;

/// <summary>
/// Represents the context of a <see cref="AdmissionReview"/>
/// </summary>
public class AdmissionReviewContext
{

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewContext"/>
    /// </summary>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to review</param>
    /// <param name="user">The information about the authenticated user that has performed the operation that is being admitted</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changed induced by the operation being admitted</param>
    public AdmissionReviewContext(AdmissionReviewRequest request)
    {
       this.Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    /// <summary>
    /// Gets the <see cref="AdmissionReviewRequest"/> to review
    /// </summary>
    public AdmissionReviewRequest Request { get; }

}
