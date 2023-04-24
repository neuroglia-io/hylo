namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to mutate resources
/// </summary>
public interface IResourceMutator
{

    /// <summary>
    /// Determines wheter or not the <see cref="IResourceMutator"/> applies to an operation performed on the specified resource kind
    /// </summary>
    /// <param name="operation">The operation being performed</param>
    /// <param name="group">The API group the resource being mutated belons to</param>
    /// <param name="version">The version of the kind of the resource being mutated</param>
    /// <param name="plural">The plural name of the kind of resource being mutated</param>
    /// <param name="namespace">The namespace the resource being mutated belongs to, if any</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceMutator"/> supports the specified resource kind</returns>
    bool AppliesTo(Operation operation, string group, string version, string plural, string? @namespace = null);

    /// <summary>
    /// Muatates the specified resource
    /// </summary>
    /// <param name="context">The context in which to perform the resource's mutation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task<AdmissionReviewResponse> MutateAsync(AdmissionReviewRequest context, CancellationToken cancellationToken = default);

}
