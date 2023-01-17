namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to mutate <see cref="V1Resource"/>s
/// </summary>
public interface IResourceMutator
{

    /// <summary>
    /// Gets the priority of the <see cref="IResourceMutator"/> in the resource mutation pipeline
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines whether or not the <see cref="IResourceMutator"/> supports the specified resource type
    /// </summary>
    /// <param name="resourceDefinition">The <see cref="V1ResourceDefinition"/> that described the resource type to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="IResourceMutator"/> supports the specified resource type</returns>
    bool SupportsResourceType(V1ResourceDefinition resourceDefinition);

    /// <summary>
    /// Muatates the specified <see cref="V1Resource"/>
    /// </summary>
    /// <param name="context">The context in which to perform the resource's mutation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task MutateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of a service used to mutate <see cref="V1Resource"/>s
/// </summary>
/// <typeparam name="TResource">The type of <see cref="V1Resource"/> to mutate</typeparam>
public interface IResourceMutator<TResource>
    : IResourceMutator
    where TResource : V1Resource, new()
{



}
