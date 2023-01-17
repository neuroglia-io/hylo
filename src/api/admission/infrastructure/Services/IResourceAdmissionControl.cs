namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to control the admission of <see cref="V1Resource"/>s
/// </summary>
public interface IResourceAdmissionControl
{

    /// <summary>
    /// Evaluates the specified <see cref="V1ResourceAdmissionReviewRequest"/>
    /// </summary>
    /// <param name="context">The <see cref="V1ResourceAdmissionReviewContext"/> of the admission evaluation to perform</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task EvaluateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default);

}
