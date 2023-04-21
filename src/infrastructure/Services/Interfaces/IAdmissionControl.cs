namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to determine whether or not to admit operations on <see cref="Resource"/>s
/// </summary>
public interface IAdmissionControl
{

    /// <summary>
    /// Reviews the specified resource operation for admission
    /// </summary>
    /// <param name="request">The request to review
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResult"/> that describes the admission review result</returns>
    Task<AdmissionReviewResult> ReviewAsync(AdmissionReviewRequest request, CancellationToken cancellationToken = default);

}