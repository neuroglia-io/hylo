namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to validate <see cref="Resource"/>s
/// </summary>
public interface IResourceValidator
{

    /// <summary>
    /// Determines wheter or not the <see cref="IResourceValidator"/> applies to an operation performed on the specified resource kind
    /// </summary>
    /// <param name="operation">The operation being performed</param>
    /// <param name="group">The API group the resource being admitted belons to</param>
    /// <param name="version">The version of the kind of the resource being admitted</param>
    /// <param name="plural">The plural name of the kind of resource being admitted</param>
    /// <param name="namespace">The namespace the resource being admitted belongs to, if any</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceValidator"/> supports the specified resource kind</returns>
    bool AppliesTo(Operation operation, string group, string version, string plural, string? @namespace = null);

    /// <summary>
    /// Validates the specified resource
    /// </summary>
    /// <param name="context">The context in which to perform the resource's validation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResponse"/> that describes the result of the operation</returns>
    Task<AdmissionReviewResponse> ValidateAsync(AdmissionReviewRequest context, CancellationToken cancellationToken = default);

}
