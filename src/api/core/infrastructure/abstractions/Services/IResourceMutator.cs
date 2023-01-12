namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to mutate <see cref="V1Resource"/>s
/// </summary>
public interface IResourceMutator
{

    /// <summary>
    /// Mutates the specified <see cref="V1Resource"/>
    /// </summary>
    /// <param name="resource">The <see cref="V1Resource"/> to mutate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="V1ResourceAdmissionEvaluationResult"/> that describes the <see cref="V1Resource"/>'s mutation result</returns>
    Task<V1ResourceAdmissionEvaluationResult> MutateAsync(V1Resource resource, CancellationToken cancellationToken = default); 

}

/// <summary>
/// Defines the fundamentals of a service used to mutate <see cref="V1Resource"/>s
/// </summary>
/// <typeparam name="TResource">The type of <see cref="V1Resource"/> to mutate</typeparam>
public interface IResourceMutator<TResource>
    where TResource : V1Resource
{

    /// <summary>
    /// Mutates the specified <see cref="V1Resource"/>
    /// </summary>
    /// <param name="resource">The <see cref="V1Resource"/> to mutate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="V1ResourceAdmissionEvaluationResult"/> that describes the <see cref="V1Resource"/>'s mutation result</returns>
    Task<V1ResourceAdmissionEvaluationResult> MutateAsync(TResource resource, CancellationToken cancellationToken = default);

}